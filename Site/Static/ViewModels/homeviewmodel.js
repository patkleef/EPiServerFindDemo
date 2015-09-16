﻿var viewModel = function () {
    var self = this;

    self.searchQuery = ko.observable("");
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();

    self.searchButtonClicked = function (data, event) {
        self.results([]);
        $.get('/api/Search?query=' + self.searchQuery(), function(data) {
            var jsonString = JSON.stringify(data, null, 4).toLocaleLowerCase();
            self.resultJson(jsonString);
            self.results(JSON.parse(jsonString.toLocaleLowerCase()));
        });
    }
}

ko.applyBindings(new viewModel(), document.getElementById("homepage"));