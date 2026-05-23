window.signalBriefLoading = (() => {
    const headings = [
        "Tuning the signal.",
        "Separating signal from noise.",
        "Scanning the stack.",
        "Finding the part that matters.",
        "Distilling the noise.",
        "Checking the pulse of the stack.",
        "Warming up the brief."
    ];

    const heading = headings[Math.floor(Math.random() * headings.length)];

    return {
        getHeading: () => heading
    };
})();
