import { spawn } from "node:child_process";

const input = "tailwind-build/btw-prebuild.css";
const unminifiedOutput = "wwwroot/blazorise.tailwind.css";
const minifiedOutput = "wwwroot/blazorise.tailwind.min.css";

const command = "tailwindcss";
const spawnOptions = {
    stdio: "inherit",
    shell: process.platform === "win32",
};

const processes = [
    spawn(command, ["-i", input, "-o", unminifiedOutput, "--watch"], spawnOptions),
    spawn(command, ["-i", input, "-o", minifiedOutput, "--watch", "--minify"], spawnOptions),
];

let exiting = false;

function stopAll(exitCode) {
    if (exiting) {
        return;
    }

    exiting = true;
    process.exitCode = exitCode;

    for (const child of processes) {
        if (child.exitCode === null && child.signalCode === null) {
            child.kill();
        }
    }
}

for (const child of processes) {
    child.on("exit", (code) => {
        if (!exiting) {
            stopAll(code ?? 0);
        }
    });

    child.on("error", () => {
        stopAll(1);
    });
}

process.on("SIGINT", () => stopAll(0));
process.on("SIGTERM", () => stopAll(0));
