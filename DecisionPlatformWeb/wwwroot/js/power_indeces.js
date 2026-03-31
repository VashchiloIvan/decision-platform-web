import POWER_INDEX_URL from './endpoints.js';

const playerTableRender = (countPlayer, container) => {
    document.querySelector('.player-infos-container').classList.remove('d-none');
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
            '<td>' + data[i].powerIndex + '</td>' +
            '</tr>';
        container.insertAdjacentHTML('beforeend', row);
    }
}

document.getElementById('playerCount').addEventListener('input', (event) => {
    const count = parseInt(event.target.value, 10);
    const container = document.querySelector('.players-table');
    container.innerHTML = '';

    if (!isNaN(count) && count >= 0) {
        playerTableRender(count, container);
    } else {
        document.querySelector('.player-infos-container').classList.add('d-none')
    }
});

document.querySelector('.calc-pi').addEventListener('click', (event) => {
    const playersWeight = document.querySelectorAll('.player-weight');
    const playersName = document.querySelectorAll('.player-name');
    const quota = document.getElementById('quota').value;
    const quotaType = document.getElementById('quotaType').value;
    const calcType = document.getElementById('calculationType').value;
    const methodType = document.getElementById('methodType').value;
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
        quota: parseFloat(quota),
        quotaType: parseFloat(quotaType),
        calculationType: parseFloat(calcType),
        methodType: parseFloat(methodType),
        players: []
    };
    console.log(data);
    for (let i = 0; i < playersName.length; i++) {
        data.players.push({
            name: playersName[i].value,
            weight: parseFloat(playersWeight[i].value)
        })
    }
    fetch(POWER_INDEX_URL, {
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
        document.querySelector('.result-container').classList.remove('d-none');
        const container = document.querySelector(".pi-info-body");
        container.innerHTML = '';
        renderPIData(result, container);
    })
});