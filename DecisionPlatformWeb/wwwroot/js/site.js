let methodsInfo = {}

$(document).ready(function () {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    // on create criteria
    $(".create-criteria-li .btn").on("click", (e) => {
        const criteriaName = $(".create-criteria-li input").val();
        const type = $(".create-criteria-li .form-select").val();
        createCriteria(criteriaName, type);
    });

    // on create alternative
    $(".math-model .alternative-table thead .btn-primary").on("click", createAlternative);

    // on change criteria relation
    $(".criteria-relation .form-select").change(fillCriteriaRelationTypeForm);

    // get from back relation types
    getCriteriaRelationsFromBack(fillCriteriaRelationVariants);

    // get method info
    getMethodInfoFromBack();

    // on select step method
    $(".methods .select-step-method").change(onStepMethodSelected);

    // on solve
    $(".get-result").click(onSolveBtnClick);

    // import export
    $(".export-xml").click(onExportXmlBtnClick);
    $(".export-json").click(onExportJsonBtnClick);
    $(".import").click(onImportBtnClick);
});

function showInvalidMessageOnClickInput(inputSelector, feedbackSelector, message) {
    $(inputSelector).addClass('is-invalid');
    let errorField = $(feedbackSelector);
    errorField.html(message);
    errorField.show();

    $(inputSelector).keyup(function (e) {
        $(inputSelector).removeClass('is-invalid');
        errorField.hide();
    })
}

// CRITERIA

class Criteria {
    constructor(name, type) {
        this.name = name;
        this.type = type;
    }

    getName() {
        return this.name;
    }

    getType() {
        return this.type;
    }
}

// === criteria list from html
function getCriteriaList() {
    let criteriaList = [];
    $(".criteria-list .criteria-li").each(function () {
        let criteriaName = $(this).children("p").html();
        let criteriaType = $(this).children("div").children("span").html();
        criteriaList.push(new Criteria(criteriaName, criteriaType));
    });
    return criteriaList;
}

// === create delete
function createCriteria(criteriaName, type) {
    // Проверка
    let message = '';
    if (criteriaName === '') {
        message = "Заполните поле";
    } else if (!criteriaName.match("[a-zA-Z0-9\s]{1,}")) {
        message = "Название критерия может содержать только латинские буквы и цифры";
    } else if (getCriteriaList().find((criteria) => criteria.getName() === criteriaName) !== undefined) {
        message = "Введите уникальное название";
    }
    if (message !== "") {
        showInvalidMessageOnClickInput(".create-criteria-li input",
            ".create-criteria-li .invalid-feedback", message);
        return;
    }

    let criteria = new Criteria(criteriaName, type);
    // Добавить критерий в список
    $(".criteria-list").append(getCriteriaListItemHtml(criteria))
    $(".criteria-li .btn-danger").on("click", deleteCriteria);

    // Очистить input
    $(".create-criteria-li input").val("");
    $(".create-criteria-li .form-select").val(1);

    // Добавить столбик в таблицу
    addCriteriaOnTable(criteria)

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

function deleteCriteria(e) {
    const li = $(e.target).parent().parent();
    const criteriaName = li.children("p").html();
    const criteriaIndex = getCriteriaList().findIndex((cr) => cr.getName() === criteriaName);

    $('.math-model .alternative-table tr').find('td:nth-child(' + (criteriaIndex + 2) + ')').remove();

    li.remove();
}

// === html helpers
function addCriteriaOnTable(criteria) {
    let mathmodelTable = $(".math-model .alternative-table");
    mathmodelTable.children("thead").children("tr").children("td:last-child").before("<td>" + criteria.getName() + "</td>");
    mathmodelTable.children("tbody").children("tr").each(function () {
        $(this).children("td:last-child").before("<td><input type=\"number\" class=\"form-control\"></td>")
    });
}

function getCriteriaListItemHtml(criteria) {
    return "<li class=\"list-group-item d-flex justify-content-between criteria-li\">\n" +
        "<p>" + criteria.getName() + "</p>\n" +
        "<div class=\"horizontal-div\">\n" +
        " <span>" + (criteria.getType() === "max" ? "max" : "min") + "</span>" +
        " <button type=\"button\" class=\"btn btn-danger btn-sm bi bi-trash\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Удалить критерий\"/>" +
        "</div>\n" +
        "</li>";
}

// ALTERNATIVE

class Alternative {
    constructor(name, marks) {
        this.name = name;
        this.marks = marks;
    }

    getName() {
        return this.name;
    }

    getMarks() {
        return this.marks;
    }
}

// === alternative list from html
function getAlternativeList() {
    let alternatives = [];
    let isValid = true;
    $(".math-model .alternative-table tbody tr").each(function () {
        const name = $(this).children("th").html();
        let marks = [];
        $(this).children("td").each(function () {
            const mark = $(this).children("input").val();
            if (mark === undefined) {
            } else if (mark === "")
                isValid = false;
            else
                marks.push(parseFloat(mark.replace(",", ".")));
        })
        alternatives.push(new Alternative(name, marks));
    });

    return isValid ? alternatives : [];
}

// === create delete
function createAlternative(e) {
    const maxAlternativeNum = $(".math-model .alternative-table tbody tr:last-child th").html();
    const criteriaCount = getCriteriaList().length;

    const newNum = isNaN(maxAlternativeNum) ? 1 : maxAlternativeNum - (-1);

    $(".math-model .alternative-table tbody").append(getAlternativeTableItemHtml(newNum, criteriaCount));
    $(".math-model .alternative-table tbody .btn-danger").on("click", deleteAlternative);

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

function createAlternativeWithMarks(name, marks) {
    $(".math-model .alternative-table tbody").append(getFilledAlternativeTableItemHtml(name, marks));
    $(".math-model .alternative-table tbody .btn-danger").on("click", deleteAlternative);

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

function deleteAlternative(e) {
    $(e.target).parent().parent().remove();
}

// === html helpers
function getAlternativeTableItemHtml(alternativeNum, criteriaCount) {
    let res = "<tr><th scope='row'>" + alternativeNum + "</th>";
    for (let i = 0; i < criteriaCount; i++)
        res += "<td><input type='number' class='form-control'></td>";
    res += "<td><button type=\"button\" class=\"btn btn-danger btn-sm bi bi-trash float-end\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Удалить альтернативу\"/></td>"
    return res + "</tr>"
}

function getFilledAlternativeTableItemHtml(name, marks) {
    let res = "<tr><th scope='row'>" + name + "</th>";
    marks.forEach(mark => {
        res += "<td><input type='number' class='form-control' value='" + mark + "'></td>";
    })
    res += "<td><button type=\"button\" class=\"btn btn-danger btn-sm bi bi-trash float-end\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Удалить альтернативу\"/></td>"
    return res + "</tr>"
}

// CRITERIA RELATION

class CriteriaRelation {
    constructor(name, criteriaRelationInfo) {
        this.name = name;
        this.info = criteriaRelationInfo;
    }

    getName() {
        return this.name
    }

    getInfo() {
        return this.info
    }
}

function getCriteriaRelationsFromBack(onSuccess) {
    $.ajax({
        url: "/criteria-relation-info",
        method: "get",
        success: function (data, textStatus, jqXHR) {
            onSuccess(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Error");
        }
    })
}

function fillCriteriaRelationVariants(variants) {
    for (let i = 0; i < variants.length; i++) {
        $(".criteria-relation .form-select").append("<option value=\"" + (i + 1) + "\">" + variants[i] + "</option>")
    }
}

function fillCriteriaRelationTypeForm(e) {
    const typeId = $(".criteria-relation .form-select").val();
    let message = "";
    switch (typeId) {
        case "1":
            message = "Введите целое число - ранг";
            break;
        case "2":
            message = "Введите число - ранг";
            break;
        case "3":
            message = "Введите число - вес критерия";
            break;
        case "4":
            message = "Заполните набор отношений критериев";
            break;
    }
    let innerHtml = '';
    if (typeId === "4") {
        innerHtml = getSimpleCriteriaRelationFormHtml();
    } else if (typeId === "1" || typeId === "2" || typeId === "3") {
        innerHtml = getWeightCriteriaRelationFormHtml();
    } else {
        innerHtml = "";
    }

    $(".criteria-relation .info-message").html(message);
    $(".criteria-relation .criteria-relation-form").html(innerHtml);
    // on add code
    $(".criteria-relation .criteria-relation-form .btn-primary").click(addRelationOnUlList);
    // set toggle 
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

function fillCriteriaRelationTypeFormFromImportedData(name, infoList) {
    let message = "";
    let typeId = "";
    switch (name) {
        case "Лексикографическое отношение":
            message = "Введите целое число - ранг";
            typeId = "1";
            break;
        case "Простое ранжирование":
            message = "Введите число - ранг";
            typeId = "2";
            break;
        case "Пропорциональный метод":
            message = "Введите число - вес критерия";
            typeId = "3";
            break;
        case "Порядковая информация":
            message = "Заполните набор отношений критериев";
            typeId = "4";
            break;
    }
    $(".criteria-relation .form-select").val(typeId);
    $(".criteria-relation .form-select").change();
    $(".criteria-relation .info-message").html(message);
    let innerHtml = '';
    if (typeId === "4") {
        innerHtml = getSimpleCriteriaRelationFormHtml();
    } else if (
        typeId === "1" || typeId === "2" || typeId === "3") {
        innerHtml = getWeightCriteriaRelationFormHtmlWithInfoList(infoList);
    } else {
        innerHtml = "";
    }

    $(".criteria-relation .criteria-relation-form").html(innerHtml);
    // on add code
    $(".criteria-relation .criteria-relation-form .btn-primary").click(addRelationOnUlList);

    if (name === "Порядковая информация")
        infoList.forEach(info => {
            addRelationOnUlListWithInfo(info);
        })

    // set toggle 
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
}

function getWeightCriteriaRelationFormHtml() {
    const criteriaList = getCriteriaList();
    let res = "<ul class=\"list-group\">";

    criteriaList.forEach(criteria => {
        res +=
            "<li class=\"list-group-item d-flex justify-content-between\">" +
            "<span class=\"input-group-text\">" + criteria.getName() + "</span>" +
            "<input type=\"number\" aria-label=\"criteria value\" class=\"form-control\">" +
            "</li>"
    })
    return res + "</ul>";
}

function getWeightCriteriaRelationFormHtmlWithInfoList(infoList) {
    let res = "<ul class=\"list-group\">";
    infoList.forEach(c => {
        res +=
            "<li class=\"list-group-item d-flex justify-content-between\">" +
            "<span class=\"input-group-text\">" + c.criteriaName + "</span>" +
            "<input type=\"number\" aria-label=\"criteria value\" class=\"form-control\" value='" + c.value + "'>" +
            "</li>"
    })
    return res + "</ul>";
}

function getSimpleCriteriaRelationFormHtml() {
    let result =
        "<ul class=\"list-group\">" +
        "<li class=\"list-group-item d-flex justify-content-between\">" +
        "<select class=\"form-select first-criteria\">";

    let selectInner = "";
    const criteriaList = getCriteriaList();
    criteriaList.forEach((criteria) => {
        selectInner += "<option value=\""
            + criteria.getName() + "\">"
            + criteria.getName() + "</option>"
    });

    result += selectInner + "</select>";
    result +=
        "<select class=\"form-select relation\">" +
        "<option selected value=\"0\">></option>" +
        "<option value=\"1\">>=</option>" +
        "<option value=\"2\">~</option>" +
        "<option value=\"3\">&lt;=</option>" +
        "<option value=\"4\">&lt;</option>" +
        "</select>";
    result += "<select class=\"form-select second-criteria\">";
    result += selectInner + "</select>";
    result +=
        "<button type=\"button\" class=\"btn btn-primary btn-sm bi bi-plus-lg\" data-toggle=\"tooltip\"" +
        "data-placement=\"top\" title=\"Добавить отношение\"></button>" +
        "</li></ul>";
    return result;
}

function addRelationOnUlList() {
    let item = "<li class=\"list-group-item d-flex justify-content-between simple-relation\">";

    const firstCriteriaName = $(".criteria-relation .criteria-relation-form .first-criteria option:selected").text();
    const relationVal = $(".criteria-relation .criteria-relation-form .relation option:selected").text();
    const secondCriteriaName = $(".criteria-relation .criteria-relation-form .second-criteria option:selected").text();

    item += "<span>" +
        "<span class='fc'>" + firstCriteriaName + "</span> " +
        "<span class='con'>" + relationVal + "</span> " +
        "<span class='sc'>" + secondCriteriaName + "</span>" +
        "</span>" +
        "<button type=\"button\" class=\"btn btn-danger btn-sm bi bi-trash\"" +
        "data-toggle=\"tooltip\" data-placement=\"top\" title=\"Удалить отношение\"/>";

    $(".criteria-relation .criteria-relation-form .list-group").append(item + "</li>");

    // on delete
    $(".criteria-relation .criteria-relation-form .btn-danger").click(deleteRelationFromList);

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });
}

function addRelationOnUlListWithInfo(info) {
    let item = "<li class=\"list-group-item d-flex justify-content-between simple-relation\">";

    const firstCriteriaName = info.first;
    const relationVal = info.constraint;
    const secondCriteriaName = info.second;

    item += "<span>" +
        "<span class='fc'>" + firstCriteriaName + "</span> " +
        "<span class='con'>" + relationVal + "</span> " +
        "<span class='sc'>" + secondCriteriaName + "</span>" +
        "</span>" +
        "<button type=\"button\" class=\"btn btn-danger btn-sm bi bi-trash\"" +
        "data-toggle=\"tooltip\" data-placement=\"top\" title=\"Удалить отношение\"/>";

    $(".criteria-relation .criteria-relation-form .list-group").append(item + "</li>");

    // on delete
    $(".criteria-relation .criteria-relation-form .btn-danger").click(deleteRelationFromList);

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });
}

function deleteRelationFromList(e) {
    $(e.target).parent().remove();
}

function getCriteriaRelationValuesFromUi() {
    const criteriaRelationName = $(".criteria-relation > .form-select option:selected").text();
    const relationVal = $(".criteria-relation > .form-select").val();

    let relArr = []
    let isValid = true;
    if (relationVal === "4") {
        $(".criteria-relation .criteria-relation-form .simple-relation > span").each(function () {
            relArr.push({
                first: $(this).children(".fc").html(),
                constraint: $(this).children(".con").html().replace("&gt;", ">").replace("&lt;", "<"),
                second: $(this).children(".sc").html()
            });
        })
    } else if (relationVal === "1" || relationVal === "2" || relationVal === "3") {
        $(".criteria-relation .criteria-relation-form .list-group .list-group-item").each(function () {
            const criteriaName = $(this).children(".input-group-text").html();
            const value = $(this).children(".form-control").val();

            if (value === "")
                isValid = false;
            else
                relArr.push({
                    criteriaName: criteriaName,
                    value: parseFloat(value.replace(",", "."))
                });
        })
    } else {
        isValid = false;
    }

    return isValid ? new CriteriaRelation(criteriaRelationName, relArr) : null;
}

// METHODS

class MethodInfo {
    constructor(methodType, methods) {
        this.type = methodType;
        this.methods = methods;
    }

    getType() {
        return this.type;
    }

    getMethods() {
        return this.methods;
    }
}

class Method {
    constructor(name, additionalMethods) {
        this.name = name;
        this.additionalMethods = additionalMethods;
    }

    getName() {
        return this.name;
    }

    getAddMethods() {
        return this.additionalMethods;
    }
}

class AdditionalMethod {
    constructor(name, value) {
        this.name = name;
        this.value = value;
    }

    getName() {
        return this.name;
    }

    getValue() {
        return this.value;
    }
}

function getMethodInfoFromBack() {
    $.ajax({
        url: "/decision-method-info",
        method: "get",
        async: false,
        success: function (data, textStatus, jqXHR) {
            methodsInfo = data;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Error");
        }
    })
}

function onStepMethodSelected(e) {
    const methodDiv = $(".methods .method-select-div");
    const val = $(e.target).val();
    methodDiv.html("");
    $(".solving").html("");

    if (val === "1") {
        methodDiv.html(getOneStepMethodInputForm());
        $(".methods .method-select-div .onestep-select").change(e => {
            const additionalMethods = addAdditionalMethodOnSelectChanged(e, methodsInfo.oneStepMethodInfo);
            $(".methods .method-select-div .additional-info").html(additionalMethods);
        })
        $(".methods .method-select-div .btn-primary").click(onAddOneStepMethodClick)

    } else if (val === "2") {
        methodDiv.html(getMultiStepMethodInputForm());
        $(".methods .method-select-div .multistep-select").change(e => {
            const additionalMethods = addAdditionalMethodOnSelectChanged(e, methodsInfo.multiStepMethodInfo);
            $(".methods .method-select-div .additional-info").html(additionalMethods);
        })
    }
}

function onAddOneStepMethodClick(e) {
    const li = getMethodLiToList();
    onAddOneStepMethodLi(li);
    $(".methods .method-select-div .onestep-select").val("non");
    $(".methods .method-select-div .onestep-select").change();
}

function onAddOneStepMethodLi(li) {
    {
        if (li !== false) {
            $(".methods .method-select-div .list-group").append(li);
            $(".methods .method-select-div .list-group .btn-danger").click((e) => {
                $(e.target).parent().parent().remove()
            })
            $(function () {
                $('[data-toggle="tooltip"]').tooltip()
            })
        }
    }
}

function getOneStepMethodInputForm() {
    let result = "";
    result += "<ul class=\"list-group\">";
    result += "<li class=\"list-group-item add-form\">";
    result +=
        "<p>Метод принятия решений</p><div class='d-flex'>" +
        "<select class=\"form-select onestep-select\">" +
        "<option value='non' selected>Choose...</option>";
    const keys = Object.keys(methodsInfo.oneStepMethodInfo);
    keys.forEach(methodName => {
        result += "<option value='" + methodName + "'>" + methodName + "</option>";
    })
    result += "</select>";
    result += "<button type=\"button\" class=\"ms-2 btn btn-primary btn-sm bi bi-plus-lg\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Добавить метод\"></button>"
    result += "</div><div class='additional-info mt-3'></div></li></ul>";
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })
    return result;
}

function getMultiStepMethodInputForm() {
    let result = "";
    result += "<p>Метод принятия решения</p>";
    result +=
        "<select class=\"form-select multistep-select mb-3\">" +
        "<option selected>Choose...</option>"
    const keys = Object.keys(methodsInfo.multiStepMethodInfo);
    keys.forEach(methodName => {
        result += "<option value='" + methodName + "'>" + methodName + "</option>"
    })
    result += "</select><div class='additional-info' />";
    return result;
}

function addAdditionalMethodOnSelectChanged(e, src) {
    const val = $(e.target).val();
    let res = "";
    if (Object.keys(src).includes(val)) {
        const additional = src[val];
        Object.keys(additional).forEach(parameter => {
            res += "<div>"
            res += "<p>" + parameter + "</p>";
            res +=
                "<select class=\"form-select mb-3\">" +
                "<option value='non' selected>Choose...</option>";
            additional[parameter].forEach(val => {
                res += "<option value='" + val + "'>" + val + "</option>"
            })
            res += "</select>"
            res += "</div>"
        })
    }
    return res;
}

function getMethodLiToList() {
    const methodName = $(".methods .method-select-div .add-form .onestep-select").val();
    if (methodName === "non")
        return false;
    let additional = [];
    let needToExit = false;
    $(".methods .method-select-div .add-form .additional-info div").each(function () {
        const paramName = $(this).children("p").html()
        const paramValue = $(this).children(".form-select").val();
        if (paramValue === "non") {
            needToExit = true;
        }
        additional.push({name: paramName, value: paramValue});
    })
    if (needToExit)
        return "";
    let res = "<li class=\"list-group-item d-flex justify-content-between method-on-li\">";
    res += "<div class='vals'>"
    res += "<p class='name fw-bold'>" + methodName + "</p>";
    additional.forEach(obj => {
        res += "<p>" + obj.name + ": " + obj.value + "</p>"
    })
    res += "</div>"
    res += "<div><button type=\"button\" class=\"btn btn-danger btn-sm bi bi-trash \" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Удалить метод\"/></div>"
    res += "</li>"
    return res;
}

function getMethodLiToListWithImport(method) {
    const methodName = method.name;
    if (methodName === "non")
        return false;
    let additional = method.additionalMethods;
    let res = "<li class=\"list-group-item d-flex justify-content-between method-on-li\">";
    res += "<div class='vals'>"
    res += "<p class='name fw-bold'>" + methodName + "</p>";
    additional.forEach(obj => {
        res += "<p>" + obj.name + ": " + obj.value + "</p>"
    })
    res += "</div>"
    res += "<div><button type=\"button\" class=\"btn btn-danger btn-sm bi bi-trash \" data-toggle=\"tooltip\" data-placement=\"top\" title=\"Удалить метод\"/></div>"
    res += "</li>"
    return res;
}

function getMethodInfo() {
    const methodType = $(".methods .select-step-method option:selected").val();
    let methods = [];
    if (methodType === "1") {
        $(".methods .method-select-div .method-on-li .vals").each(function () {
            let methodName;
            let additionalMethods = [];
            $(this).children("p").each(function () {
                const val = $(this).html();
                if ($(this).hasClass("name")) {
                    methodName = val;
                } else {
                    const split = val.split(": ");
                    additionalMethods.push(new AdditionalMethod(split[0], split[1]));
                }
            })
            methods.push(new Method(methodName, additionalMethods));
        })
    } else if (methodType === "2") {
        const methodName = $(".methods .method-select-div .multistep-select option:selected").text();
        let additionalMethods = [];
        $(".methods .method-select-div .additional-info div").each(function () {
            const key = $(this).children("p").html();
            const value = $(this).children("select").children("option:selected").html();
            additionalMethods.push(new AdditionalMethod(key, value));
        });
        methods.push(new Method(methodName, additionalMethods));
    } else
        return null;
    return new MethodInfo(methodType, methods);
}

// SOLVE
function onSolveBtnClick(e) {
    console.log("solve start");
    $.ajax({
        url: "solve-task",
        type: "POST",
        data: JSON.stringify(buildSolveModel()),
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            $(".solving").html("<div class=\"d-flex justify-content-center\">" +
                "  <div class=\"spinner-border\" role=\"status\">" +
                "    <span class=\"visually-hidden\">Loading...</span>" +
                "  </div>" +
                "</div>")
        },
        success: function (response) {
            $(".solving").html(response);
        },
        error: function () {
            alert("error");
        }
    });
}

function buildSolveModel() {
    return {
        criteriaList: getCriteriaList(),
        alternativeList: getAlternativeList(),
        criteriaRelation: getCriteriaRelationValuesFromUi(),
        methodInfo: getMethodInfo()
    };
}

// import
function onImportBtnClick(e) {
    var fileDialog = $('<input id="import-file-upload-qweqwe" type="file">');
    fileDialog.click();
    fileDialog.on("change", onImportFileSelected);
    return false;
}

function onImportFileSelected(e) {
    var formData = new FormData();
    formData.append('file', $(e.target)[0].files[0]);
    console.log(formData);

    $.ajax({
        type: "POST",
        url: 'import',
        cache: false,
        contentType: false,
        processData: false,
        async: true,
        data: formData,
        dataType: 'json',
        beforeSend: function () {
            const importButton = $(".import");
            importButton.removeClass("bi")
            importButton.removeClass("bi bi-arrow-bar-down")
            importButton.prop("disabled", true)
            importButton.html("<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"></span>" + importButton.html())
        },
        complete: function () {
            const importButton = $(".import");
            importButton.addClass("bi")
            importButton.addClass("bi bi-arrow-bar-down")
            importButton.prop("disabled", false)
            $(importButton).children(".spinner-border").remove()
            $(".solving").children().remove()
        },
        success: function (response) {
            // Criteria list
            $(".criteria-list .bi-trash").click();
            response.criteriaList.forEach(c => {
                createCriteria(c.name, c.type);
            })

            // Alternatives 
            $(".alternative-table .bi-trash").click();
            response.alternativeList.forEach(a => {
                createAlternativeWithMarks(a.name, a.marks)
            })

            // Criteria Relations 
            $(".criteria-relation-form .bi-trash").click();
            if (response.criteriaRelation !== null)
                fillCriteriaRelationTypeFormFromImportedData(response.criteriaRelation.name, response.criteriaRelation.info)
            else {
                const item = $(".criteria-relation .form-select");
                item.val("-100");
                item.change();
            }

            // Methods
            if (response.methodInfo !== null) {
                $(".methods .select-step-method").val(response.methodInfo.type);
                $(".methods .select-step-method").change();
                
                if (response.methodInfo.type === "1") {
                    response.methodInfo.methods.forEach(m => {
                        onAddOneStepMethodLi(getMethodLiToListWithImport(m));
                    })
                } else if (response.methodInfo.type === "2") {
                    if (response.methodInfo.methods.length > 0) {
                        $(".multistep-select").val(response.methodInfo.methods[0].name)
                        $(".multistep-select").change()
                        const additionalMethods = response.methodInfo.methods[0].additionalMethods;
                        $(".methods .method-select-div .additional-info").html(additionalMethods);
                    }
                }
            } else {
                const item = $(".methods .form-select");
                item.val("-100");
                item.change();
            }
        },
        error: function () {
            // alert("onImportFileSelected")
        }
    });
}

// export 
function onExportXmlBtnClick(e) {
    onExportFileSelected("export-xml", "model.xml");
}

function onExportJsonBtnClick(e) {
    onExportFileSelected("export-json", "model.json");
}

function onExportFileSelected(url, filename) {
    $.ajax({
        method: "POST",
        url: url,
        data: JSON.stringify(buildSolveModel()),
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
        error: function () {
            // alert("QweQWEQWe")
        }
    })
}