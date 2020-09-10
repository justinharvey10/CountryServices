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
    '.valid-text { color: green; padding-left: 20px }'
  ]
})
export class HomeComponent implements OnInit {

  validCodes$: Observable<string[]>;
  allValidCodesList: string[];
  inputCode: string;
  countrySummary: CountrySummary;

  constructor(private countryLookupService: CountryLookupService) { }

  ngOnInit() {
    this.validCodes$ = this.countryLookupService.getValidCountryCodes();
    this.validCodes$.subscribe(validCodes => {
      this.allValidCodesList = validCodes;
      console.log("Received " + validCodes.length + " valid codes");
    });
  }
  onChange() {
    this.countrySummary = null;

    if (this.inputCode.length > 1 && this.isValidCountryCode()) {
      
      this.countryLookupService.getCountryDetails(this.inputCode).subscribe({
        next: countrySummaryIn => this.countrySummary = countrySummaryIn,
        error: () => console.log('Failed to retrieve country summary for code: ' + this.inputCode),
        complete: () => console.log('Completed call to retrieve country summary')
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
