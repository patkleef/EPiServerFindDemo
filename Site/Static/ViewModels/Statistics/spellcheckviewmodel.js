
var spellCheckViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();
    self.useJavascript = ko.observable(false);

    self.inputFieldKeyPressed = function (data, event) {
        if (event.currentTarget.value != "") {
            search(event.currentTarget.value);
        }
        else {
            self.results([]);
            self.resultJson("");
        }
    }

    function search(query) {
        if (self.useJavascript()) {
            $.get('/find_v2/_spellcheck?query=' + query + '&size=5', function (data) {
                self.resultJson(JSON.stringify(data, null, 4));
                self.results(data.hits);
            });
        }
        else {
            $.get('/api/Search/SpellCheck?query=' + query, function (data) {
                var jsonString = JSON.stringify(data, null, 4).toLocaleLowerCase();
                self.resultJson(jsonString);
                self.results(JSON.parse(jsonString).hits);
            });
        }
    }
}

ko.applyBindings(new spellCheckViewModel(), document.getElementById("statistics-spellcheck"));