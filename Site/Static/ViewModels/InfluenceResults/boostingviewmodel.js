
var boostingViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();
    self.useAutoBoosting = ko.observable(false);
    self.decayScale = ko.observable(14);
    self.decayOffset = ko.observable(1);
    self.decayShape = ko.observable(1);
    self.hitBoostScale = ko.observable(2);
    self.hitBoostOffset = ko.observable(0);

    self.decayOrigin = ko.observable(new Date().toISOString().substr(0, 10));
    self.decayMinimum = ko.observable(0);

    self.searchButtonClicked = function (data, event) {
        if (self.searchQuery() != "") {
            var json = {
                useAutoBoosting: self.useAutoBoosting(),
                decayScale: self.decayScale(),
                decayOffset: self.decayOffset(),
                decayShape: self.decayShape(),
                decayOrigin: self.decayOrigin(),
                decayMinimum: self.decayMinimum(),
                hitBoostScale: self.hitBoostScale(),
                hitBoostOffset: self.hitBoostOffset(),
                query: self.searchQuery(),
                ticks: $.now()
        };

            $.get('/api/Search/Boosting', json, function (data) {
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

ko.applyBindings(new boostingViewModel(), document.getElementById("results-boosting"));