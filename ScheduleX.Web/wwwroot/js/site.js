window.downloadFile = (fileName, base64Data) => {
    try {
        const link = document.createElement('a');
        link.download = fileName;
        link.href = "data:application/octet-stream;base64," + base64Data;

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } catch (e) {
        console.error("Download error:", e);
        alert("Failed to download file");
    }
};

//login wrong attempts
function getAttempts() {
    return parseInt(localStorage.getItem("attempts") || "0");
}

function setAttempts(val) {
    localStorage.setItem("attempts", val);
}

function resetAttempts() {
    localStorage.removeItem("attempts");
}

function setLock(seconds) {
    localStorage.setItem("lockUntil", Date.now() + (seconds * 1000));
}

function getLockRemaining() {
    const lockUntil = localStorage.getItem("lockUntil");
    if (!lockUntil) return 0;

    const diff = Math.floor((lockUntil - Date.now()) / 1000);
    return diff > 0 ? diff : 0;
}

function setRobotCheck(val) {
    localStorage.setItem("robotCheck", val);
}
function getRobotCheck() {
    return localStorage.getItem("robotCheck") === "true";
}
function clearRobotCheck() {
    localStorage.removeItem("robotCheck");
}
