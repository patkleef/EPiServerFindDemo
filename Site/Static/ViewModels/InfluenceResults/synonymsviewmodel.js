﻿var synonymsViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();

    self.searchButtonClicked = function (data, event) {
        if (self.searchQuery() != "") {
            $.get('/api/Search/Synonyms?query=' + self.searchQuery(), function (data) {
                var jsonString = JSON.stringify(data, null, 4);
                self.resultJson(jsonString);
                self.results(data);
            });
        }
        else {
            self.results([]);
            self.resultJson("");
        }
    }
}

ko.applyBindings(new synonymsViewModel(), document.getElementById("results-synonyms"));