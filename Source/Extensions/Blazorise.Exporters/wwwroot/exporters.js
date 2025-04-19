

export function copyStringToClipboard(stringToCopy) {
  if (navigator.clipboard) {
    navigator.clipboard.writeText(stringToCopy);
  }
}

export function exportToFile(data, fileName, mimeType) {
  // Convert .NET byte array to Uint8Array
  const uint8Array = new Uint8Array(data);

  // Create Blob with specified MIME type
  const blob = new Blob([uint8Array], { type: mimeType });

  // Create temporary URL
  const url = URL.createObjectURL(blob);

  // Create hidden anchor element
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName;
  document.body.appendChild(a);

  // Trigger download
  a.click();

  // Cleanup
  document.body.removeChild(a);
  URL.revokeObjectURL(url);

  return 1; // Success
}

