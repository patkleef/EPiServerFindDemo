var morelikeViewModel = function () {
    var self = this;

    self.searchQuery = ko.observable();
    self.results = ko.observableArray([]);
    self.resultJson = ko.observable();
    self.articles = ko.observableArray([]);
    self.selectedArticle = ko.observable(0);

    self.minimumDocumentFrequency = ko.observable(5);
    self.maximumDocumentFrequency = ko.observable(25);
    self.percentTermsToMatch = ko.observable(30);
    self.minimumTermFrequency = ko.observable(2);
    self.minimumWordLength = ko.observable(0);
    self.maximumWordLength = ko.observable(0);
    self.maximumQueryTerms = ko.observable(25);
    self.stopWords = ko.observable("");

    init();

    self.searchButtonClicked = function (data, event) {
        if (self.searchQuery() != "") {
            var json = {
                articleId: self.selectedArticle().Id,
                minimumDocumentFrequency: self.minimumDocumentFrequency(),
                maximumDocumentFrequency: self.maximumDocumentFrequency(),
                percentTermsToMatch: self.percentTermsToMatch(),
                minimumTermFrequency: self.minimumTermFrequency(),
                minimumWordLength: self.minimumWordLength(),
                maximumWordLength: self.maximumWordLength(),
                maximumQueryTerms: self.maximumQueryTerms(),
                stopWords: self.stopWords()
            };

            $.get('/api/Search/RelatedArticles', json, function (data) {
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

    function init() {
        $.get('/api/Search/AllArticles', function (data) {
            self.articles(data);
        });
    }
}

ko.applyBindings(new morelikeViewModel(), document.getElementById("results-morelike"));