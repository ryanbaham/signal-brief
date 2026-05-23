window.signalBriefDeployment = {
    formatDate: (deployedAtUtc) => {
        const deployedAt = new Date(deployedAtUtc);

        return new Intl.DateTimeFormat(undefined, {
            year: "numeric",
            month: "long",
            day: "numeric"
        }).format(deployedAt);
    },
    formatLocalTime: (deployedAtUtc) => {
        const deployedAt = new Date(deployedAtUtc);

        return new Intl.DateTimeFormat(undefined, {
            dateStyle: "full",
            timeStyle: "long"
        }).format(deployedAt);
    }
};
