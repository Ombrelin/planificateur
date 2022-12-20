const addDate = (date: Date | null = null) => {
    const dates = document.querySelector("#dates");
    const newDate = document.createElement("div");
    newDate.classList.add("columns","mb-0");
    newDate.innerHTML = `
        <div class="field mb-0 column is-three-quarters">
            <div class="control">
                <input class="input" value="${date?.toISOString().slice(0, 16)}" type="datetime-local" name="dates[]"/>
            </div>
        </div>
        <div class="column  is-flex is-justify-content-end">
            <button type="button" onclick="removeDate(this)" class="button">
                <span class="icon">
                  <i class="fas fa-trash"></i>
                </span>
            </button>
        </div>
    `;
    dates?.insertAdjacentElement( "afterend", newDate);
}

const addDateRange = () => {
    const startDate = new Date(document.querySelector("#range-start-date")?.getAttribute("value") ?? "");
    const endDate = new Date(document.querySelector("#range-start-date")?.getAttribute("value") ?? "");
    
    for(let currentDate: Date = startDate ; currentDate <= endDate ; currentDate.setDate(currentDate.getDate() + 1)){
        addDate(currentDate);
    }
}
 
const removeDate = (element: HTMLElement) => {
    element.parentElement?.parentElement?.remove();
}