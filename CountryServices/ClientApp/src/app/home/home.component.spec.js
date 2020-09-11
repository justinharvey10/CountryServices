"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
require("jasmine");
var home_component_1 = require("./home.component");
var forms_1 = require("@angular/forms");
var countrylookup_service_1 = require("../services/countrylookup.service");
var rxjs_1 = require("rxjs");
describe('HomeComponent', function () {
    var component;
    var fixture;
    var mockCountryLookupService;
    beforeEach(function () {
        testing_1.TestBed.configureTestingModule({
            providers: [countrylookup_service_1.CountryLookupService],
            declarations: [home_component_1.HomeComponent],
            imports: [forms_1.FormsModule]
        })
            .compileComponents();
        mockCountryLookupService = jasmine.createSpyObj(['getValidCountryCodes', 'getCountryDetails']);
        mockCountryLookupService.getValidCountryCodes.and.returnValue(new rxjs_1.Observable());
        mockCountryLookupService.getCountryDetails.and.returnValue(new rxjs_1.Observable());
        // Configure the component with another set of Providers
        testing_1.TestBed.overrideComponent(home_component_1.HomeComponent, { set: { providers: [{ provide: countrylookup_service_1.CountryLookupService, useValue: mockCountryLookupService }] } });
        fixture = testing_1.TestBed.createComponent(home_component_1.HomeComponent);
        component = fixture.componentInstance;
        component.allValidCodesList = ['GB'];
        fixture.detectChanges();
    });
    it('should create as expected', function () {
        expect(component).toBeTruthy();
        expect(component.allValidCodesList).toContain('GB');
    });
    it('No list, not Invalid', function () {
        component.allValidCodesList = null;
        component.inputCode = 'g';
        expect(component.isInvalid()).toBeFalsy();
    });
    it('1 character is not Invalid', function () {
        component.inputCode = 'g';
        expect(component.isInvalid()).toBeFalsy();
    });
    it('2 characters Valid', function () {
        component.inputCode = 'gb';
        expect(component.isInvalid()).toBeFalsy();
    });
    it('2 uppercase characters Valid', function () {
        component.inputCode = 'GB';
        expect(component.isInvalid()).toBeFalsy();
    });
    it('2 characters Invalid', function () {
        component.inputCode = 'gc';
        expect(component.isInvalid()).toBeTruthy();
    });
    it('OnChange and input valid calls service once', function () {
        component.inputCode = 'gb';
        component.onChange();
        expect(mockCountryLookupService.getCountryDetails).toHaveBeenCalledTimes(1);
    });
    it('OnChange and one character input not valid calls service never', function () {
        component.inputCode = 'g';
        component.onChange();
        expect(mockCountryLookupService.getCountryDetails).toHaveBeenCalledTimes(0);
    });
    it('OnChange and two character input not valid calls service never', function () {
        component.inputCode = 'gc';
        component.onChange();
        expect(mockCountryLookupService.getCountryDetails).toHaveBeenCalledTimes(0);
    });
    it('1 character is not valid country code', function () {
        component.inputCode = 'g';
        expect(component.isValidCountryCode()).toBeFalsy();
    });
    it('2 characters valid country code', function () {
        component.inputCode = 'gb';
        expect(component.isValidCountryCode()).toBeTruthy();
    });
    it('2 uppercase characters valid country code', function () {
        component.inputCode = 'GB';
        expect(component.isValidCountryCode()).toBeTruthy();
    });
    it('2 characters invalid country code', function () {
        component.inputCode = 'gc';
        expect(component.isValidCountryCode()).toBeFalsy();
    });
    it('getInputUpper returns upper', function () {
        component.inputCode = 'aaa';
        var upper = component.getInputUpper();
        expect(upper).toBe('AAA');
    });
    it('getInputUpper handles null', function () {
        component.inputCode = null;
        var upper = component.getInputUpper();
        expect(upper).toBe(null);
    });
});
//# sourceMappingURL=home.component.spec.js.map