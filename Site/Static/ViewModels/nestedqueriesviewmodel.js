var nestedQueriesViewModel = function () {
    var self = this;

    self.companies = ko.observable();
    self.facetCountries = ko.observable();
    self.facetAges = ko.observable();
    self.selectedCompany = ko.observable("");
    self.selectedCountry = ko.observable("");
    self.selectedAge = ko.observable("");
    self.results = ko.observableArray([]);

    init();

    self.searchButtonClicked = function() {
        $.get('/api/Search/SearchCompanies', function (data) {
            self.countries(data);
        });
    }

    self.facetCountryClicked = function (object, sender) {
        if (object.Term === self.selectedCountry()) {
            self.selectedCountry("");
        }
        else {
            self.selectedCountry(object.Term);
        }
        $.get('/api/Search/SearchCompanies', { country: self.selectedCountry(), age: self.selectedAge() }, function (data) {
            self.results(data);
        });
    }

    self.facetAgeClicked = function (object, sender) {
        if (object.Term === self.selectedAge()) {
            self.selectedAge("");
        }
        else {
            self.selectedAge(object.Key);
        }
        
        $.get('/api/Search/SearchCompanies', { country: self.selectedCountry(), age: self.selectedAge() }, function (data) {
            self.results(data);
        });
    }

    self.formatAgeFacet = function (age) {
        var ageNumber = parseInt(age);
        return ageNumber + " - " + (ageNumber + 10);
    }
    
    function init() {
        $.get('/api/Search/CompanyCountryFacet', function (data) {
            self.facetCountries(data.Countries);
            self.facetAges(data.Ages);
            self.results(data.Companies);
        });
    }
}

ko.applyBindings(new nestedQueriesViewModel(), document.getElementById("results-nestedqueries"));