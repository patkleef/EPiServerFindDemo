
var boostingWithWeightsViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();
    self.useBoostingWithWeights = ko.observable(false);
    self.searchTitle = ko.observable(1);
    self.searchText = ko.observable(1);
    self.searchSummary = ko.observable(1);
    self.searchAttachment = ko.observable(1);

    self.searchButtonClicked = function (data, event) {
        if (self.searchQuery() != "") {
            var json = {
                useBoostingWithWeights: self.useBoostingWithWeights(),
                searchTitle: self.searchTitle(),
                searchText: self.searchText(),
                searchSummary: self.searchSummary(),
                searchAttachment: self.searchAttachment(),
                query: self.searchQuery()
        };

            $.get('/api/Search/BoostingWithWeights', json, function (data) {
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

ko.applyBindings(new boostingWithWeightsViewModel(), document.getElementById("results-boosting"));