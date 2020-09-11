import { Component, OnInit } from '@angular/core';
import { CountryLookupService, CountrySummary } from '../services/countrylookup.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: [
    '.country-input { width: 50px; }',
    '.alert {color: red}',
    '.row-label { width: 100px; font-weight: bold }',
    '.valid-text { color: green; padding-left: 20px }',
    '.page-error { color: red }'
  ]
})
export class HomeComponent implements OnInit {

  validCodes$: Observable<string[]>;
  allValidCodesList: string[];
  inputCode: string;
  countrySummary: CountrySummary;
  listServiceErrorMessage: string;
  detailsServiceErrorMessage: string;

  constructor(private countryLookupService: CountryLookupService) { }

  ngOnInit() {
    this.validCodes$ = this.countryLookupService.getValidCountryCodes();
    this.validCodes$.subscribe({
      next: validCodes => this.allValidCodesList = validCodes,
      error: () => this.listServiceErrorMessage = 'There was a problem loading the page, please try again later'
    });
  }
  onChange() {
    this.countrySummary = null;
    this.detailsServiceErrorMessage = null;

    if (this.inputCode.length > 1 && !this.isInvalid()) {
      
      this.countryLookupService.getCountryDetails(this.inputCode).subscribe({
        next: countrySummaryIn => this.countrySummary = countrySummaryIn,
        error: () => this.detailsServiceErrorMessage = 'There was a problem getting country details, please try again later'
      });
    }
  }
  isInvalid() {
    return (this.allValidCodesList &&
      this.inputCode && this.inputCode.length > 1 &&
      this.allValidCodesList.indexOf(this.getInputUpper()) === -1);
  }
  isValidCountryCode() {
    return this.allValidCodesList && this.allValidCodesList.indexOf(this.getInputUpper()) > -1;
  }

  getInputUpper() {
    return this.inputCode && this.inputCode.toUpperCase();
  }
}
