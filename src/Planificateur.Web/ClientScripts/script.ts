const addDate = () => {
    const dates = document.querySelector("#dates");
    const newDate = document.createElement("div");
    newDate.classList.add("columns","mb-0");
    newDate.innerHTML = `
        <div class="field mb-0 column is-three-quarters">
            <div class="control">
                <input class="input" type="date" name="dates[]"/>
            </div>
        </div>
        <div class="column  is-flex is-justify-content-end">
            <button type="button" onclick="removeDate(this)" class="button">
                <span class="icon">
                  <i class="delete"></i>
                </span>                
            </button>
        </div>
    `;
    dates?.insertAdjacentElement( "afterend", newDate);
}

const removeDate = (element: HTMLElement) => {
    element.parentElement?.parentElement?.remove();
}