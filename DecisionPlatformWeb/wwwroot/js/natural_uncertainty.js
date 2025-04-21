let nuConfig = {}

$(document).ready(function () {
    fetchMethodInfo("nu-config");

    const mathModelTable = new Table("table.nu-mathmodel-table");
    const uncertaintyUl = new UlList("ul.nu-uncertainty-list", "button.nu-add-uncertainty", mathModelTable, false);
    const alternativeUl = new UlList("ul.nu-alternative-list", "button.nu-add-alternative", mathModelTable, true);

    const criteriaList = new CriteriaList('.nu-criterias-select', 'button.nu-add-criteria', nuConfig);

    const solver = new Solver("button.nu-solve", "div.nu-solving", criteriaList, mathModelTable);
})

class Table {
    constructor(selector) {
        this.$table = $(selector);
        this.rows = new Set(); // Множество для хранения названий строк
        this.columns = new Set(); // Множество для хранения названий столбцов
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
            const $cell = $('<td>').html(`<input type="number" class="form-control" data-row="${rowName}" data-column="${columnName}" />`);
            $row.append($cell);
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
            const $cell = $('<td>').html(
                `<input type="number" class="form-control" data-row="${rowName}" data-column="${columnName}" value="${value}" />`
            );
            $row.append($cell);
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

        this.$table.find('thead tr').append($('<th>').text(columnName));

        this.rows.forEach(rowName => {
            const $cell = $('<td>').html(`<input type="number" class="form-control" data-row="${rowName}" data-column="${columnName}" />`);
            this.$table.find(`tr[data-row-name="${rowName}"]`).append($cell);
        });
    }

    // Удаление строки
    removeRow(rowName) {
        if (!this.rows.has(rowName)) {
            console.warn(`Строка с названием "${rowName}" не найдена.`);
            return;
        }

        this.rows.delete(rowName);
        this.$table.find(`tr[data-row-name="${rowName}"]`).remove();
    }

    // Удаление столбца
    removeColumn(columnName) {
        if (!this.columns.has(columnName)) {
            console.warn(`Столбец с названием "${columnName}" не найден.`);
            return;
        }

        this.columns.delete(columnName);

        this.$table.find('thead th').each(function () {
            if ($(this).text() === columnName) {
                $(this).remove();
            }
        });

        this.$table.find(`input[data-column="${columnName}"]`).closest('td').remove();
    }

    // Получение данных из таблицы
    getTableData() {
        const uncertainties = this.getUncertainties(); // Получаем неопределенности
        const alternatives = this.getAlternatives(); // Получаем альтернативы

        return {
            uncertainties,
            alternatives,
        };
    }

    // Получение списка неопределенностей
    getUncertainties() {
        const uncertainties = [];
        this.$table.find('thead th').each(function (index) {
            if (index > 0) { // Пропускаем первый столбец (названия альтернатив)
                uncertainties.push($(this).text());
            }
        });
        return uncertainties;
    }

    // Получение альтернатив и их значений
    getAlternatives() {
        const alternatives = {};

        this.$table.find('tbody tr').each(function () {
            const $row = $(this);
            const altName = $row.find('th').text(); // Название альтернативы (первая ячейка)
            const values = [];

            // Собираем значения для альтернативы
            $row.find('td').each(function () {
                const value = parseFloat($(this).find('input').val());
                values.push(value);
            });

            alternatives[altName] = values;
        });

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
        this.fillSelect(); // Заполняем select
        this.bindEvents(); // Навешиваем обработчики событий
    }

    // Заполнение select
    fillSelect() {
        const $select = this.$list.find('.form-select');
        $select.empty(); // Очищаем select

        // Добавляем option "Choose..."
        $select.append($('<option>', {
            value: '',
            text: 'Choose...',
            selected: true,
        }));

        // Добавляем option из конфига
        this.config.criterias.forEach(criteria => {
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
        const selectedCriteria = this.config.criterias.find(criteria => criteria.method === selectedMethod);

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
        const selectedCriteria = this.config.criterias.find(criteria => criteria.method === selectedMethod);

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

        // Формируем итоговый объект
        const data = {
            mathModel: tableData,
            criterias: criteriaData,
        };

        console.log(data);

        $.ajax({
            url: '/nu-solve',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            beforeSend: function() {
                solveResult.append(`<div class="d-flex justify-content-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>`);
            },
            success: function(response) {
                solveResult.empty();
                solveResult.append(response);
            },
            error: function(jqXHR, textStatus, errorThrown) {
                console.error('Ошибка при отправке данных:', textStatus, errorThrown);
                alert('Произошла ошибка при отправке данных. Пожалуйста, попробуйте позже.');
                solveResult.empty();
            }
        });
    }
}

class ImportExportHandler {
    constructor(importBtnSelector, exportJsonBtnSelector, exportXmlBtnSelector, endpoints, lists) {
        this.$importBtn = $(importBtnSelector);
        this.$exportJsonBtn = $(exportJsonBtnSelector);
        this.$exportXmlBtn = $(exportXmlBtnSelector);
        this.importEndpoint = endpoints.import;
        this.exportJsonEndpoint = endpoints.exportJson;
        this.exportXmlEndpoint = endpoints.exportXml;

        // Ссылки на списки
        this.uncertaintyUl = lists.uncertaintyUl;
        this.alternativeUl = lists.alternativeUl;
        this.criteriaList = lists.criteriaList;

        this.initEvents();
    }

    initEvents() {
        this.$importBtn.on('click', () => this.handleImport());
        this.$exportJsonBtn.on('click', () => this.handleExport(this.exportJsonEndpoint));
        this.$exportXmlBtn.on('click', () => this.handleExport(this.exportXmlEndpoint));
    }

    handleImport() {
        const $fileInput = $('<input type="file" accept=".json,.xml" style="display: none;">');
        $('body').append($fileInput);

        $fileInput.on('change', (event) => {
            const file = event.target.files[0];
            if (!file) return;

            const formData = new FormData();
            formData.append("file", file);

            $.ajax({
                url: this.importEndpoint,
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: (data) => this.populateInterface(data),
                error: () => alert("Ошибка при импорте файла. Проверьте формат и повторите попытку."),
                complete: () => $fileInput.remove()
            });
        });

        $fileInput.click();
    }

    handleExport(endpoint) {
        window.open(endpoint, '_blank');
    }

    populateInterface(data) {
        // Очистка текущих данных
        this.clearLists();

        const model = data.mathModel;
        const criterias = data.criterias;

        // Импорт неопределенностей
        if (Array.isArray(model.uncertainties)) {
            model.uncertainties.forEach(u => this.uncertaintyUl.addItem(u));
        }

        // Импорт альтернатив
        if (model.alternatives && typeof model.alternatives === 'object') {
            for (const [altName, marks] of Object.entries(model.alternatives)) {
                this.alternativeUl.addItem(altName, marks);
            }
        }
        
        // Импорт критериев
        if (Array.isArray(criterias)) {
            criterias.forEach(c => {
                const method = c.criteria;
                const params = c.parameters || [];

                const configCriteria = this.criteriaList.config.criterias.find(k => k.method === method);
                if (!configCriteria) return;

                const $newItem = $('<li>', {
                    class: 'list-group-item d-flex justify-content-between align-items-center',
                }).append(
                    $('<div>', { class: 'vals' }).append(
                        $('<p>', {
                            class: 'name fw-bold',
                            text: configCriteria.name,
                            attr: { 'data-method': method },
                        }),
                        ...(params.map(param => {
                            const configParam = this.criteriaList.config.parameters.find(p => p.key === Object.keys(param)[0]);
                            const key = Object.keys(param)[0];
                            const value = param[key];
                            return $('<p>', {
                                text: `${configParam ? configParam.name : key}: ${value}`,
                                attr: {
                                    'data-parameter': key,
                                    'data-parameter-value': value,
                                }
                            });
                        }))
                    ),
                    $('<div>').append(
                        $('<button>', {
                            type: 'button',
                            class: 'btn btn-danger btn-sm bi bi-trash',
                            title: 'Удалить метод',
                            'aria-label': 'Удалить метод'
                        })
                    )
                );

                this.criteriaList.$list.append($newItem);
            });
        }
    }

    clearLists() {
        this.uncertaintyUl.$list.empty();
        this.uncertaintyUl.items.clear();
        this.alternativeUl.$list.empty();
        this.alternativeUl.items.clear();
        this.criteriaList.$list.find('li:gt(0)').remove(); // Оставить первую li с селектом
        this.criteriaList.table.clear(); // если есть метод очистки таблицы
    }
}
