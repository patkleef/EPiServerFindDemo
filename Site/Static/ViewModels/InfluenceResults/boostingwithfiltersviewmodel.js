
var boostingWithFiltersViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();
    self.useBoostingWithFilters = ko.observable(false);
    self.startDate = ko.observable(new Date().toISOString().substr(0, 10));
    self.endDate = ko.observable(new Date().toISOString().substr(0, 10));
    self.boostFactor = ko.observable(1);

    self.searchButtonClicked = function (data, event) {
        if (self.searchQuery() != "") {
            var json = {
                query: self.searchQuery(),
                useBoostingWithFilters: self.useBoostingWithFilters(),
                startDate: self.startDate(),
                endDate: self.endDate(),
                boostFactor: self.boostFactor()
        };

            $.get('/api/Search/BoostingWithFilters', json, function (data) {
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

ko.applyBindings(new boostingWithFiltersViewModel(), document.getElementById("results-boosting"));