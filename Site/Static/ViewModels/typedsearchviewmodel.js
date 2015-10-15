var typedSearchViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable("");
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();

    self.searchButtonClicked = function (data, event) {
        self.results([]);
        $.get('/api/TypedSearch?query=' + self.searchQuery(), function (data) {
            var jsonString = JSON.stringify(data, null, 4);
            self.resultJson(jsonString);
            self.results(data);
        });
    }
}
ko.applyBindings(new typedSearchViewModel(), document.getElementById("typed-search"));