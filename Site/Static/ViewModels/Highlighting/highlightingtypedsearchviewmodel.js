var highlightingTypedSearchViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();

    self.searchButtonClicked = function (data, event) {
        if (self.searchQuery() != "") {
            $.get('/api/Search/HighlightingTypedSearch?query=' + self.searchQuery(), function (data) {
                var jsonString = JSON.stringify(data, null, 4).toLocaleLowerCase();
                self.resultJson(jsonString);
                self.results(JSON.parse(jsonString));
            });
        }
        else {
            self.results([]);
            self.resultJson("");
        }
    }
}

ko.applyBindings(new highlightingTypedSearchViewModel(), document.getElementById("result-highlight-typedsearch"));