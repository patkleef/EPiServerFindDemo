var highlightingUnifiedSearchViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.numberOfFragments = ko.observable(2);
    self.fragmentSize = ko.observable(200);
    self.preTag = ko.observable("<em>");
    self.postTag = ko.observable("</em>");
    self.concatentation = ko.observable(" ... ");

    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();

    self.searchButtonClicked = function (data, event) {
        if (self.searchQuery() != "") {
            var json = {
                query: self.searchQuery(),
                numberOfFragments: self.numberOfFragments(),
                fragmentSize: self.fragmentSize(),
                preTag: self.preTag(),
                postTag: self.postTag(),
                concatentation: self.concatentation()
            };
            $.get('/api/Search/Highlight', json, function (data) {
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

ko.applyBindings(new highlightingUnifiedSearchViewModel(), document.getElementById("result-highlight"));