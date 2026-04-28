let databasePromise;

window.OpenDatabase = async function () {
    if (!databasePromise) {
        databasePromise = new Promise((resolve, reject) => {
            const request = indexedDB.open("LogicGateDB", 1);

            request.onupgradeneeded = (event) => {
                const db = event.target.result;
                if (!db.objectStoreNames.contains("FileStorage")) {
                    db.createObjectStore("FileStorage", { keyPath: "id" });
                }
            };
            request.onsuccess = () => resolve(request.result);
            request.onerror = () => reject(request.error);
        });
    }
    return databasePromise;
}

window.UpsertAsync = async function(key, streamRef) {
    const database = await window.OpenDatabase();
    const arrayBuffer = await streamRef.arrayBuffer();
    const transaction = database.transaction("FileStorage", "readwrite");
    const objectStore = transaction.objectStore("FileStorage");

    return new Promise((resolve, reject) => {
        const request = objectStore.put({ id: key, content: new Uint8Array(arrayBuffer) });
        request.onsuccess = () => resolve();
        request.onerror = () => reject(request.error);
    });
};

window.ReadAsync = async function(key) {
    const database = await window.OpenDatabase();
    const transaction = database.transaction("FileStorage", "readonly");
    const objectStore = transaction.objectStore("FileStorage");

    return new Promise((resolve, reject) => {
        const request = objectStore.get(key);
        request.onsuccess = () => {
            if (request.result && request.result.content) resolve(request.result.content);
            else resolve(null);
        };
        request.onerror = () => reject(request.error);
    });
};

window.DeleteAsync = async function(key) {
    const database = await window.OpenDatabase();
    const transaction = database.transaction("FileStorage", "readwrite");
    const objectStore = transaction.objectStore("FileStorage");

    return new Promise((resolve, reject) => {
        const request = objectStore.delete(key);
        request.onsuccess = () => resolve();
        request.onerror = () => reject(request.error);
    });
};
