import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CountryLookupService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  //Call server to get all codes
  getValidCountryCodes() {
    return this.http.get<string[]>(
      this.baseUrl + 'api/CountryCodeLookup/GetValidCountryCodes');
  }

  //Call server to get a single country details
  getCountryDetails(code: string) {
    return this.http.get<CountrySummary>(
      this.baseUrl + 'api/CountryCodeLookup/GetCountryDetails/' + code);
  }
}

export class CountrySummary {
  countryName: string;
  region: string;
  capitalCity: string;
  longitude: string;
  latitude: string;
}
