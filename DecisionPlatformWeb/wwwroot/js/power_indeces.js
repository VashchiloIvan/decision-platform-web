const playerTableRender = (countPlayer, container) => {
    for (let i = 0; i < countPlayer; i++) {
        const row = `<div class="w-100 d-flex justify-content-between mb-1">` +
            `<div class="col-5"><input type="text" class="form-control player-name" /></div>` +
            `<div class="col-5"><input type="number" class="form-control player-weight" /></div>` + `</div>`;
        container.insertAdjacentHTML('beforeend', row);
    }
};

const renderPIData = (data, container) => {
    for (let i = 0; i < data.length; i++) {
        const row = '<tr>' + '<td>' + data[i].name + '</td>' +
            '<td>' + data[i].weight + '</td>' +
            '<td>' + data[i].index + '</td>' +
            '</tr>';
        container.insertAdjacentHTML('beforeend', row);
    }
}

document.getElementById("playerCount").addEventListener("input", (event) => {
    console.log(1)
    const count = parseInt(event.target.value, 10);
    const container = document.querySelector(".players-table");
    container.innerHTML = '';

    if (!isNaN(count) && count >= 0) {
        playerTableRender(count, container);
    }
});

document.querySelector(".calc-pi-ss").addEventListener("click", (event) => {
    console.log(2);
    const playersWeight = document.querySelectorAll(".player-weight");
    const playersName = document.querySelectorAll(".player-name");
    const quota = document.getElementById("quota").value;
    const quotaType = document.getElementById("quotaType").value;
    const calcType = document.getElementById("calcType").value;
    if (quota <= 0) {
        return;
    }
    if (playersWeight.length === 0) {
        return;
    }
    if (playersName.length === 0) {
        return;
    }
    const data = {
        quota: quota,
        quotaType: quotaType,
        calcType: calcType,
        players: []
    };
    for (let i = 0; i < playersName.length; i++) {
        data.players.push({
            name: playersName[i].value,
            weight: playersWeight[i].value
        })
    }
    fetch('/PowerIndeces/calculate/ss', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    }).then(response => {
        if (!response.ok) {
            throw new Error("Ошибка при отправке запроса: " + response.status);
        }
        return response.json();
    }).then(result => {
        console.log("Результат:", result);
        const container = document.querySelector(".pi-info-body");
        container.innerHTML = '';
        renderPIData(result, container);
    })
});

document.querySelector(".calc-pi-banz").addEventListener("click", (event) => {
    console.log(2);
    const playersWeight = document.querySelectorAll(".player-weight");
    const playersName = document.querySelectorAll(".player-name");
    const quota = document.getElementById("quota").value;
    const quotaType = document.getElementById("quotaType").value;
    const calcType = document.getElementById("calcType").value;
    if (quota <= 0) {
        return;
    }
    if (playersWeight.length === 0) {
        return;
    }
    if (playersName.length === 0) {
        return;
    }
    const data = {
        quota: quota,
        quotaType: quotaType,
        calcType: calcType,
        players: []
    };
    for (let i = 0; i < playersName.length; i++) {
        data.players.push({
            name: playersName[i].value,
            weight: playersWeight[i].value
        })
    }
    fetch('/PowerIndeces/calculate/banz', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    }).then(response => {
        if (!response.ok) {
            throw new Error("Ошибка при отправке запроса: " + response.status);
        }
        return response.json();
    }).then(result => {
        console.log("Результат:", result);
        const container = document.querySelector(".pi-info-body");
        container.innerHTML = '';
        renderPIData(result, container);
    })
});