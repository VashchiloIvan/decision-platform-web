let nuConfig = {}

$(document).ready(function () {
    fetchMethodInfo("nu-config");

    const mathModelTable = new Table("table.nu-mathmodel-table");

    const uncertaintyUl = new UlList("ul.nu-uncertainty-list", "button.nu-add-uncertainty", mathModelTable, false);
    const alternativeUl = new UlList("ul.nu-alternative-list", "button.nu-add-alternative", mathModelTable, true);

    const criteriaList = new CriteriaList('.nu-criterias-select', 'button.nu-add-criteria', nuConfig);

    const solver = new Solver("button.nu-solve", "div.nu-solving", criteriaList, mathModelTable);

    $('#toggleProbabilities').on('change', function () {
        if (this.checked) {
            mathModelTable.addProbabilities();
            criteriaList.withProbabilities();
        } else {
            mathModelTable.removeProbabilities();
            criteriaList.withoutProbabilities();
        }
    });

    $("a.nu-export-xml").click(e => onExportXmlBtnClick(criteriaList, mathModelTable))
    $("a.nu-export-json").click(e => onExportJsonBtnClick(criteriaList, mathModelTable))
})

class Table {
    constructor(selector) {
        this.$table = $(selector);
        this.rows = new Set();
        this.columns = new Set();
        this.withProbabilities = false;
    }

    // Добавление строки
    addRow(rowName) {
        if (this.rows.has(rowName)) {
            console.warn(`Строка с названием "${rowName}" уже существует.`);
            return;
        }

        this.rows.add(rowName);

        const $row = $('<tr>').attr('data-row-name', rowName);
        $row.append($('<th>').attr('scope', 'row').text(rowName));

        this.columns.forEach(columnName => {
            // Добавляем ячейку с значением
            const $valueCell = $('<td>').html(
                `<input type="number" class="form-control" data-row="${rowName}" data-column="${columnName}" />`
            );
            $row.append($valueCell);

            // Если включены вероятности, добавляем ячейку для вероятности
            if (this.withProbabilities) {
                const $probCell = $('<td class="probability">').html(
                    `<input type="number" class="form-control probability-input" 
                     data-row="${rowName}" data-column="p(${columnName})" min="0" max="1" step="0.01" />`
                );
                $row.append($probCell);
            }
        });

        this.$table.find('tbody').append($row);
    }

    addRowWithMarks(rowName, marks = []) {
        if (this.rows.has(rowName)) {
            console.warn(`Строка с названием "${rowName}" уже существует.`);
            return;
        }

        this.rows.add(rowName);

        const $row = $('<tr>').attr('data-row-name', rowName);
        $row.append($('<th>').attr('scope', 'row').text(rowName));

        let index = 0;
        this.columns.forEach(columnName => {
            const value = marks[index] != null ? marks[index] : '';
            const $valueCell = $('<td>').html(
                `<input type="number" class="form-control" data-row="${rowName}" data-column="${columnName}" value="${value}" />`
            );
            $row.append($valueCell);

            if (this.withProbabilities) {
                const $probCell = $('<td class="probability">').html(
                    `<input type="number" class="form-control probability-input" 
                     data-row="${rowName}" data-column="p(${columnName})" min="0" max="1" step="0.01" />`
                );
                $row.append($probCell);
            }
            index++;
        });

        this.$table.find('tbody').append($row);
    }

    // Добавление столбца
    addColumn(columnName) {
        if (this.columns.has(columnName)) {
            console.warn(`Столбец с названием "${columnName}" уже существует.`);
            return;
        }

        this.columns.add(columnName);

        // Добавляем заголовок столбца
        const $header = $('<th>').text(columnName);
        this.$table.find('thead tr').append($header);

        // Если включены вероятности, добавляем заголовок для вероятности
        if (this.withProbabilities) {
            $header.after($('<th class="probability">').text(`p(${columnName})`));
        }

        // Добавляем ячейки для всех строк
        this.rows.forEach(rowName => {
            const $row = this.$table.find(`tr[data-row-name="${rowName}"]`);
            const $valueCell = $('<td>').html(
                `<input type="number" class="form-control" data-row="${rowName}" data-column="${columnName}" />`
            );
            $row.append($valueCell);

            if (this.withProbabilities) {
                const $probCell = $('<td class="probability">').html(
                    `<input type="number" class="form-control probability-input" 
                     data-row="${rowName}" data-column="p(${columnName})" min="0" max="1" step="0.01" />`
                );
                $row.append($probCell);
            }
        });
    }

    // Удаление строки (остается без изменений)
    removeRow(rowName) {
        if (!this.rows.has(rowName)) {
            console.warn(`Строка с названием "${rowName}" не найдена.`);
            return;
        }

        this.rows.delete(rowName);
        this.$table.find(`tr[data-row-name="${rowName}"]`).remove();
    }

    // Удаление столбца (обновлено для работы с вероятностями)
    removeColumn(columnName) {
        if (!this.columns.has(columnName)) {
            console.warn(`Столбец с названием "${columnName}" не найден.`);
            return;
        }

        this.columns.delete(columnName);

        // Удаляем основной заголовок столбца
        this.$table.find('thead th').each(function () {
            if ($(this).text() === columnName || $(this).text() === `p(${columnName})`) {
                $(this).remove();
            }
        });

        // Удаляем все соответствующие ячейки
        this.$table.find(`input[data-column="${columnName}"], input[data-column="p(${columnName})"]`)
            .closest('td').remove();
    }

    addProbabilities() {
        if (this.withProbabilities) {
            return; // Вероятности уже добавлены
        }

        this.withProbabilities = true;

        // Добавляем столбцы вероятностей для каждого существующего столбца
        const columns = Array.from(this.columns);
        columns.forEach((columnName, index) => {
            // Вставляем новый заголовок для вероятности после текущего столбца
            const $header = this.$table.find('thead th').filter((i, el) => $(el).text() === columnName);
            $header.after($('<th class="probability">').text(`p(${columnName})`));

            // Добавляем ячейки вероятностей для каждой строки
            this.rows.forEach(rowName => {
                const $row = this.$table.find(`tr[data-row-name="${rowName}"]`);
                const $inputCell = $row.find(`td input[data-column="${columnName}"]`).closest('td');
                $inputCell.after($('<td class="probability">').html(
                    `<input type="number" class="form-control probability-input" 
                 data-row="${rowName}" data-column="p(${columnName})" min="0" max="1" step="0.01" />`
                ));
            });
        });
    }

    removeProbabilities() {
        if (!this.withProbabilities) {
            return; // Вероятностей нет
        }

        this.withProbabilities = false;

        // Удаляем все заголовки столбцов вероятностей
        this.$table.find('th.probability').remove();

        // Удаляем все ячейки вероятностей
        this.$table.find('td.probability').remove();
    }

    // Остальные методы без изменений
    getTableData() {
        const uncertainties = this.getUncertainties();
        const alternatives = this.getAlternatives();

        return {
            uncertainties,
            alternatives,
        };
    }

    // Получение списка неопределенностей
    getUncertainties() {
        return [...this.columns];
    }
    
    // Получение альтернатив и их значений
    getAlternatives() {
        const alternatives = {};
        const rows = this.$table.find('tbody tr');

        for (let i = 0; i < rows.length; i++) {
            const $row = $(rows[i]);
            const altName = $row.find('th').text();
            const values = [];
            const cells = $row.find('td');

            if (this.withProbabilities) {
                // Обрабатываем пары ячеек (значение + вероятность)
                for (let j = 0; j < cells.length; j += 2) {
                    const $markCell = $(cells[j]);
                    const $probCell = $(cells[j + 1]);

                    const markInput = $markCell.find('input');
                    const probInput = $probCell?.find('input'); // Опциональная цепочка на случай нечетного количества
                    
                    const markValue = markInput.length ? parseFloat(markInput.val()) || 0 : 0;
                    const probValue = probInput?.length ? parseFloat(probInput.val()) || 0 : 0;
                    
                    values.push({
                        mark: markValue,
                        probability: probValue
                    });
                }
            } else {
                // Обрабатываем все ячейки как отдельные значения
                for (let j = 0; j < cells.length; j++) {
                    const $cell = $(cells[j]);
                    const $input = $cell.find('input');

                    if ($input.length) {
                        values.push({
                            mark: parseFloat($input.val()) || 0,
                            probability: 1
                        });
                    }
                }
            }

            alternatives[altName] = values;
        }

        return alternatives;
    }
}

class UlList {
    constructor(listSelector, addButtonSelector, tableInstance, isRowType) {
        this.$list = $(listSelector);
        this.$addButton = $(addButtonSelector);
        this.table = tableInstance;
        this.isRowType = isRowType;
        this.items = new Set();
        this.initEvents();
    }

    initEvents() {
        this.$addButton.on('click', () => this.addItemFromUi());
    }

    // 📌 Новый метод — из UI
    addItemFromUi() {
        const inputValue = this.$addButton.closest('li').find('input').val().trim();

        if (!inputValue) {
            this.showValidationError('Пожалуйста, заполните это поле');
            return;
        }

        if (this.items.has(inputValue)) {
            this.showValidationError('Имя должно быть уникальным');
            return;
        }

        if (!inputValue.match(/^[а-яА-Яa-zA-Z0-9\s]+$/)) {
            this.showValidationError('Название может содержать только буквы и цифры');
            return;
        }

        this.addItem(inputValue); // Вызываем универсальный метод
        this.$addButton.closest('li').find('input').val('');
    }

    addItem(name, marks = []) {
        const $newItem = $(`
            <li class="list-group-item">
                <div class="d-flex justify-content-between">
                    <span>${name}</span>
                    <button type="button" class="btn btn-danger btn-sm bi bi-trash" data-toggle="tooltip" title="Удалить" aria-label="Удалить"></button>
                </div>
            </li>
        `);

        this.$list.append($newItem);
        this.items.add(name);

        if (this.isRowType) {
            if (marks.length > 0) {
                this.table.addRowWithMarks(name, marks);
            } else {
                this.table.addRow(name);
            }
        } else {
            this.table.addColumn(name);
        }

        $newItem.find('.bi-trash').on('click', () => this.removeItem($newItem, name));
    }

    removeItem($item, name) {
        $item.remove();
        this.items.delete(name);

        if (this.isRowType) {
            this.table.removeRow(name);
        } else {
            this.table.removeColumn(name);
        }
    }

    showValidationError(message) {
        const $input = this.$addButton.closest('li').find('input');
        const $feedback = this.$addButton.closest('li').find('.invalid-feedback');

        $feedback.text(message);
        $input.addClass('is-invalid');
        $feedback.show();

        $input.on('keyup', () => {
            $input.removeClass('is-invalid');
            $feedback.hide();
        });
    }
}

function fetchMethodInfo(url) {
    $.ajax({
        url: url,
        method: "get",
        async: false,
        success: function (data) {
            nuConfig = data;

            console.log("got config:", nuConfig)
        }, error: function (jqXHR, textStatus, errorThrown) {
            console.error("Ошибка при получении данных:", errorThrown);
            alert("Произошла ошибка при загрузке данных. Пожалуйста, попробуйте позже.");

            return null;
        }
    })
}

class CriteriaList {
    constructor(listSelector, addButtonSelector, config) {
        this.$list = $(listSelector); // Список <ul>
        this.$addButton = $(addButtonSelector); // Кнопка добавления
        this.config = config; // Конфигурация
        this.init();
    }

    // Инициализация
    init() {
        this.withoutProbabilities(); // По умолчанию используем критерии без вероятностей
        this.bindEvents(); // Навешиваем обработчики событий
    }

    // Заполнение select
    fillSelect(criterias) {
        const $select = this.$list.find('.form-select');
        $select.empty(); // Очищаем select

        // Добавляем option "Choose..."
        $select.append($('<option>', {
            value: '',
            text: 'Choose...',
            selected: true,
        }));

        // Добавляем option из конфига
        criterias.forEach(criteria => {
            $select.append($('<option>', {
                value: criteria.method,
                text: criteria.name,
            }));
        });
    }

    // Навешивание обработчиков событий
    bindEvents() {
        // Обработчик изменения select
        this.$list.on('change', '.form-select', (event) => this.handleSelectChange(event));

        // Обработчик нажатия на кнопку добавления
        this.$addButton.on('click', () => this.addListItem());

        // Обработчик нажатия на кнопку удаления
        this.$list.on('click', '.bi-trash', (event) => this.removeListItem(event));
    }

    // Обработчик изменения select
    handleSelectChange(event) {
        const $select = $(event.target);
        const selectedMethod = $select.val();
        const $additionalInfo = $select.closest('.list-group-item').find('.additional-info');
        $additionalInfo.empty(); // Очищаем блок дополнительных параметров

        if (selectedMethod === '') return;

        // Находим выбранный критерий
        let selectedCriteria = this.config.withProbabilityCriterias.find(criteria => criteria.method === selectedMethod);
        if (selectedCriteria === undefined) {
            selectedCriteria = this.config.withoutProbabilityCriterias.find(criteria => criteria.method === selectedMethod);
        }

        // Если у критерия есть параметры, добавляем их
        if (selectedCriteria.parameters) {
            selectedCriteria.parameters.forEach(paramKey => {
                const param = this.config.parameters.find(p => p.key === paramKey);
                if (param) {
                    this.addParameterInput($additionalInfo, param);
                }
            });
        }
    }

    // Добавление input для параметра
    addParameterInput($container, param) {
        const $inputGroup = $('<div>', {
            class: 'input-group mt-2',
        });

        const $input = $('<input>', {
            type: param.type === 'number' ? 'number' : 'text',
            class: 'form-control',
            placeholder: param.name,
            required: true,
            attr: {
                'data-parameter-value': '', // Добавляем атрибут для значения параметра
            },
        });

        const $feedback = $('<div>', {
            class: 'invalid-feedback',
            text: `Пожалуйста, заполните поле "${param.name}".`,
        });

        $inputGroup.append($input, $feedback);
        $container.append($inputGroup);

        // Обновляем атрибут data-parameter-value при изменении input
        $input.on('input', () => {
            $input.attr('data-parameter-value', $input.val());
        });
    }

    // Добавление нового элемента в список
    addListItem() {
        const $select = this.$list.find('.form-select');
        const selectedMethod = $select.val();

        if (selectedMethod === '') {
            return;
        }

        // Находим выбранный критерий
        let selectedCriteria = this.config.withProbabilityCriterias.find(criteria => criteria.method === selectedMethod);
        if (selectedCriteria === undefined) {
            selectedCriteria = this.config.withoutProbabilityCriterias.find(criteria => criteria.method === selectedMethod);
        }

        // Проверяем, заполнены ли все input[type="number"]
        let isValid = true;
        this.$list.find('input[type="number"]').each(function () {
            if (!$(this).val()) {
                isValid = false;
                $(this).addClass('is-invalid');
                $(this).siblings('.invalid-feedback').show();

                $(this).on('keyup', () => {
                    $(this).removeClass('is-invalid');
                    $(this).siblings('.invalid-feedback').hide();
                });
            }
        });

        if (!isValid) {
            return;
        }

        // Создаем новый элемент списка
        const $newItem = $('<li>', {
            class: 'list-group-item d-flex justify-content-between align-items-center',
        }).append(
            $('<div>', {
                class: 'vals',
            }).append(
                $('<p>', {
                    class: 'name fw-bold',
                    text: selectedCriteria.name,
                    attr: {
                        'data-method': selectedCriteria.method, // Добавляем атрибут data-method
                    },
                }),
                // Добавляем параметры, если они есть
                ...(selectedCriteria.parameters
                    ? selectedCriteria.parameters.map(paramKey => {
                        const param = this.config.parameters.find(p => p.key === paramKey);
                        return param
                            ? $('<p>', {
                                text: `${param.name}: ${$select.closest('.list-group-item').find(`input[placeholder="${param.name}"]`).val()}`,
                                attr: {
                                    'data-parameter': param.key,
                                    'data-parameter-value': $select.closest('.list-group-item').find(`input[placeholder="${param.name}"]`).val(),
                                },
                            })
                            : null;
                    })
                    : [])
            ),
            $('<div>').append(
                $('<button>', {
                    type: 'button',
                    class: 'btn btn-danger btn-sm bi bi-trash',
                    'data-toggle': 'tooltip',
                    'data-placement': 'top',
                    title: 'Удалить метод',
                    'aria-label': 'Удалить метод',
                })
            )
        );

        // Добавляем элемент в список
        this.$list.append($newItem);

        // Сбрасываем select на "Choose..." и очищаем дополнительные поля
        $select.val('').trigger('change');
    }

    // Удаление элемента из списка
    removeListItem(event) {
        const $button = $(event.target);
        $button.closest('li').remove(); // Удаляем родительский <li>
    }

    // Метод для получения данных
    getData() {
        const data = [];

        this.$list.find('li').each((index, li) => {
            if (index === 0) {
                return
            }

            const $li = $(li);
            const criteriaName = $li.find('.name').attr('data-method'); // Получаем метод из data-method
            const parameters = [];

            // Собираем параметры
            $li.find('p[data-parameter]').each((i, p) => {
                const $p = $(p);
                const key = $p.attr('data-parameter'); // Ключ параметра
                const value = $p.attr('data-parameter-value'); // Значение параметра

                const criteriaParameter = {key, value}

                parameters.push(criteriaParameter);
            });

            data.push({
                criteriaName,
                parameters,
            });
        });

        return data;
    }

    // Переключение на критерии с вероятностями
    withProbabilities() {
        // Очищаем список (кроме первого элемента, который содержит select)
        this.$list.find('li:not(:first)').remove();
        // Заполняем select критериями с вероятностями
        this.fillSelect(this.config.withProbabilityCriterias);
    }

    // Переключение на критерии без вероятностей
    withoutProbabilities() {
        // Очищаем список (кроме первого элемента, который содержит select)
        this.$list.find('li:not(:first)').remove();
        // Заполняем select критериями без вероятностей
        this.fillSelect(this.config.withoutProbabilityCriterias);
    }
}

class Solver {
    constructor(buttonSelector, solveResultSelector, criteriaListInstance, tableInstance) {
        this.$button = $(buttonSelector); // Кнопка
        this.$solveResult = $(solveResultSelector) // div для результата ПР
        this.criteriaList = criteriaListInstance; // Экземпляр CriteriaList
        this.table = tableInstance; // Экземпляр Table
        this.init();
    }

    // Инициализация
    init() {
        this.bindEvents(); // Навешиваем обработчики событий
    }

    // Навешивание обработчиков событий
    bindEvents() {
        this.$button.on('click', () => this.solve());
    }

    // Обработчик нажатия на кнопку
    solve() {
        const criteriaData = this.criteriaList.getData(); // Получаем данные из CriteriaList
        const tableData = this.table.getTableData(); // Получаем данные из Table
        const solveResult = this.$solveResult;

        if (criteriaData.length === 0) {
            return;
        }

        const isChecked = $('#toggleProbabilities').prop('checked');

        const data = {
            withProbabilities: isChecked,
            mathModel: tableData,
            criterias: criteriaData,
        };

        console.log(data);

        $.ajax({
            url: '/nu-solve',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            beforeSend: function () {
                solveResult.append(`<div class="d-flex justify-content-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>`);
            },
            success: function (response) {
                solveResult.empty();
                solveResult.append(response);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Ошибка при отправке данных:', textStatus, errorThrown);
                alert('Произошла ошибка при отправке данных. Пожалуйста, попробуйте позже.');
                solveResult.empty();
            }
        });
    }
}

function onExportXmlBtnClick(criteriaList, table) {
    onExportFileSelected("/nu-export-xml", "model.xml", criteriaList, table);
}

function onExportJsonBtnClick(criteriaList, table) {
    onExportFileSelected("/nu-export-json", "model.json", criteriaList, table);
}

function onExportFileSelected(url, filename, criteriaList, table) {
    const criteriaData = criteriaList.getData(); // Получаем данные из CriteriaList
    const tableData = table.getTableData(); // Получаем данные из Table

    if (criteriaData.length === 0) {
        return;
    }

    const isChecked = $('#toggleProbabilities').prop('checked');

    const data = {
        withProbabilities: isChecked,
        mathModel: tableData,
        criterias: criteriaData,
    };
    
    $.ajax({
        method: "POST",
        url: url,
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            var blob = new Blob([response], {type: "application/octet-stream"});
            var url2 = URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = url2;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
        },
        error: function () {}
    })
}
