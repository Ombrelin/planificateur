const addDate = (date: Date | null = null) => {
    const dates = document.querySelector("#dates");
    const newDate = document.createElement("div");
    newDate.classList.add("is-flex", "mb-1");
    newDate.innerHTML = `
        <div class="field mb-0 p-0 pr-1">
            <div class="control">
                <input class="input" value="${date?.toISOString().slice(0, 16)}" type="datetime-local" name="dates[]"/>
            </div>
        </div>
        <div class="p-0 pl-1 is-flex is-justify-content-end">
            <button type="button" onclick="removeDate(this)" class="button">
                <span class="icon">
                  <i class="fas fa-trash"></i>
                </span>
            </button>
        </div>
    `;
    dates?.appendChild(newDate);
}

const addDateRange = () => {
    const startDate = new Date((document.querySelector("#range-start-date") as HTMLInputElement).value);
    const endDate = new Date((document.querySelector("#range-end-date") as HTMLInputElement).value);

    startDate.setHours(12);
    endDate.setHours(12);
    
    const firstDateInput = document.querySelector("#first-date") as HTMLInputElement;

    if (firstDateInput.value === "") {
        firstDateInput.value = startDate.toISOString().slice(0, 16);
        startDate.setDate(startDate.getDate() + 1);
    }

    for (let currentDate: Date = startDate; currentDate <= endDate; currentDate.setDate(currentDate.getDate() + 1)) {
        addDate(currentDate);
    }
}

const fillTimezoneInForm = () => {
    const timezoneInput = document.querySelector("#timezone") as HTMLInputElement;
    timezoneInput.value = Intl.DateTimeFormat().resolvedOptions().timeZone;
}

const removeDate = (element: HTMLElement) => {
    element.parentElement?.parentElement?.remove();
}

const deleteVote = async (pollId: string, voteId: string) => {
    await fetch(`/api/polls/${pollId}/votes/${voteId}`, {
        method: "DELETE"
    });
    
    location.reload();
}