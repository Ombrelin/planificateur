const getLocaleDate = (date: Date) => new Date(date.getTime() - (new Date().getTimezoneOffset() * 60000));

const getLocaleISOString = (date: Date) => getLocaleDate(date).toISOString();

const addDate = (date: Date | null = null) => {
    const dates = document.querySelector("#dates");
    const newDate = document.createElement("div");
    newDate.classList.add("is-flex", "mb-1");
    newDate.innerHTML = `
        <div class="field mb-0 p-0 pr-1">
            <div class="control">
                <input class="input" value="${date === null ? "" : getLocaleISOString(date).slice(0, 16)}" type="datetime-local" name="dates[]"/>
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
    const time = (document.querySelector("#range-time") as  HTMLInputElement).value;
    const splitTime = time.split(":");
    
    const timeHours = Number.parseInt(splitTime[0]);
    const timeMinutes = Number.parseInt(splitTime[1]);
    
    startDate.setHours(timeHours);
    startDate.setMinutes(timeMinutes);
    endDate.setHours(timeHours);
    endDate.setMinutes(timeMinutes);
    const firstDateInput = document.querySelector("#first-date") as HTMLInputElement;
    
    if (firstDateInput.value === "") {
        firstDateInput.value = getLocaleISOString(startDate).slice(0, 16);
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

const saveVoterName = () => {
    const voterNameField = document.querySelector("#voter-name") as HTMLInputElement;
    
    localStorage.setItem("voterName", voterNameField.value);
}

const loadVoterName = () => {
    const voterName = localStorage.getItem("voterName");
    if(voterName){
        const voterNameField = document.querySelector("#voter-name") as HTMLInputElement;
        voterNameField.value = voterName;
    }
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

const fillLocalizedDate = (td: HTMLElement) => {
    const utcDate = new Date(
        Date.UTC(
            Number.parseInt(td.dataset.year ?? "0"), 
            Number.parseInt(td.dataset.month ?? "0") + 1,
            Number.parseInt(td.dataset.day ?? "0"), 
            Number.parseInt(td.dataset.hour ?? "0"),
            Number.parseInt(td.dataset.minute ?? "0")
        )
    );
    const dateOptions: Intl.DateTimeFormatOptions = {
        weekday: 'long', 
        year: 'numeric',
        month: '2-digit',
        day: 'numeric'
    };
    
    const timeOptions:  Intl.DateTimeFormatOptions = {
        hour: "numeric",
        minute: "numeric",
        hourCycle: "h24"
    };
    
    td.innerHTML = `
        ${Intl.DateTimeFormat(Intl.DateTimeFormat().resolvedOptions().locale, dateOptions).format(utcDate)}
        <br>
        ${Intl.DateTimeFormat(Intl.DateTimeFormat().resolvedOptions().locale, timeOptions).format(utcDate)}
    `;
}

const loadLocalDates = () => {
    const tds = document.querySelectorAll(".date-cell");
    for(const td of tds){
        fillLocalizedDate(td as HTMLElement);
    }
}