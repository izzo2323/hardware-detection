const SECTIONS = [
  {
    title: "Operating System",
    dataKey: "operatingSystem",
    errorKey: "Operating System",
    kind: "single",
    fields: [["name", "Name"], ["version", "Version"], ["architecture", "Architecture"], ["installDate", "Install Date"]]
  },
  {
    title: "Motherboard",
    dataKey: "motherboard",
    errorKey: "Motherboard",
    kind: "single",
    fields: [["manufacturer", "Manufacturer"], ["model", "Model"], ["serialNumber", "Serial Number"]]
  },
  {
    title: "BIOS",
    dataKey: "bios",
    errorKey: "BIOS",
    kind: "single",
    fields: [["manufacturer", "Manufacturer"], ["version", "Version"], ["releaseDate", "Release Date"]]
  },
  {
    title: "CPU",
    dataKey: "cpus",
    errorKey: "CPU",
    kind: "list",
    fields: [["name", "Name"], ["manufacturer", "Manufacturer"], ["cores", "Cores"], ["threads", "Threads"], ["maxClockSpeedMHz", "Max Clock (MHz)"]]
  },
  {
    title: "RAM",
    dataKey: "ram",
    errorKey: "RAM",
    kind: "single",
    fields: [
      ["totalPhysicalMemoryGB", "Total Physical Memory (GB)", v => v.toFixed(2)],
      ["totalMemoryFromModulesGB", "Total from Modules (GB)", v => v.toFixed(2)],
      ["memoryModules", "Memory Modules"]
    ]
  },
  {
    title: "Disk Drives",
    dataKey: "diskDrives",
    errorKey: "Disk Drives",
    kind: "list",
    fields: [["model", "Model"], ["manufacturer", "Manufacturer"], ["sizeGB", "Size (GB)", v => v.toFixed(2)]]
  },
  {
    title: "Graphics Adapters",
    dataKey: "graphicsAdapters",
    errorKey: "Graphics Adapters",
    kind: "list",
    fields: [["adapterRamMB", "Adapter RAM (MB)", v => v.toFixed(2)], ["driverVersion", "Driver Version"], ["videoMode", "Video Mode"]]
  },
  {
    title: "Network Adapters",
    dataKey: "networkAdapters",
    errorKey: "Network Adapters",
    kind: "list",
    fields: [["name", "Name"], ["macAddress", "MAC Address"], ["adapterType", "Type"], ["connected", "Connected"]]
  },
  {
    title: "Monitors",
    dataKey: "monitors",
    errorKey: "Monitors",
    kind: "list",
    fields: [["name", "Name"], ["screenWidth", "Width"], ["screenHeight", "Height"]]
  },
  {
    title: "USB Devices",
    dataKey: "usbDevices",
    errorKey: "USB Devices",
    kind: "list",
    fields: [["name", "Name"], ["description", "Description"], ["deviceId", "Device ID"], ["caption", "Caption"]]
  },
  {
    title: "USB Controllers",
    dataKey: "usbControllers",
    errorKey: "USB Controllers",
    kind: "list",
    fields: [["name", "Name"], ["description", "Description"], ["deviceId", "Device ID"], ["driverVersion", "Driver Version"]]
  }
];

const content = document.getElementById("content");
const generatedAtEl = document.getElementById("generated-at");
const refreshBtn = document.getElementById("refresh-btn");

function el(tag, className, text) {
  const node = document.createElement(tag);
  if (className) node.className = className;
  if (text !== undefined) node.textContent = text;
  return node;
}

function renderEntry(item, fields) {
  const entry = el("div", "entry");
  const dl = document.createElement("dl");
  for (const [key, label, formatter] of fields) {
    const raw = item[key];
    const value = formatter && typeof raw === "number" ? formatter(raw) : raw;
    const dt = el("dt", null, label);
    const dd = el("dd", null, value === undefined || value === null ? "N/A" : String(value));
    dl.append(dt, dd);
  }
  entry.append(dl);
  return entry;
}

function renderSection(section, snapshot) {
  const card = el("div", "card");
  card.append(el("h2", null, section.title));

  const error = snapshot.sectionErrors && snapshot.sectionErrors[section.errorKey];
  if (error) {
    card.append(el("p", "error", `Error: ${error}`));
    return card;
  }

  if (section.kind === "single") {
    const item = snapshot[section.dataKey];
    if (!item) {
      card.append(el("p", "empty", "No data reported."));
    } else {
      card.append(renderEntry(item, section.fields));
    }
    return card;
  }

  const items = snapshot[section.dataKey] || [];
  if (items.length === 0) {
    card.append(el("p", "empty", "No devices found."));
  } else {
    for (const item of items) {
      card.append(renderEntry(item, section.fields));
    }
  }
  return card;
}

async function loadHardware() {
  refreshBtn.disabled = true;
  refreshBtn.textContent = "Refreshing…";

  try {
    const res = await fetch("/api/hardware");
    if (!res.ok) throw new Error(`Server responded with ${res.status}`);
    const snapshot = await res.json();

    content.innerHTML = "";

    if (!snapshot.isWindows) {
      const warning = el("p", "error", snapshot.sectionErrors?.General || "This utility only reports hardware data on Windows.");
      content.append(warning);
    }

    for (const section of SECTIONS) {
      content.append(renderSection(section, snapshot));
    }

    generatedAtEl.textContent = `Generated ${new Date(snapshot.generatedAt).toLocaleString()}`;
  } catch (err) {
    content.innerHTML = "";
    content.append(el("p", "error", `Failed to load hardware data: ${err.message}`));
  } finally {
    refreshBtn.disabled = false;
    refreshBtn.textContent = "Refresh";
  }
}

refreshBtn.addEventListener("click", loadHardware);
loadHardware();
