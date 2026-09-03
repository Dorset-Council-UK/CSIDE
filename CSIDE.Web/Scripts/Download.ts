type DotNetStreamReferenceLike = {
    arrayBuffer(): Promise<ArrayBuffer>;
};

export async function downloadFileFromStream(fileName: string, contentStreamReference: DotNetStreamReferenceLike) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    const url = URL.createObjectURL(blob);

    try {
        const anchorElement = document.createElement("a");
        anchorElement.href = url;
        anchorElement.download = fileName;
        anchorElement.style.display = "none";
        document.body.appendChild(anchorElement);
        anchorElement.click();
        anchorElement.remove();
    }
    finally {
        URL.revokeObjectURL(url);
    }
}