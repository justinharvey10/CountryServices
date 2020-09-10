import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CountryLookupService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  getHttpOptions() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
  }

  getValidCountryCodes() {
    return this.http.get<string[]>(
      this.baseUrl + 'api/CountryCodeLookup/GetValidCountryCodes', this.getHttpOptions());
  }

  getCountryDetails(code: string) {
    return this.http.get<CountrySummary>(
      this.baseUrl + 'api/CountryCodeLookup/GetCountryDetails/' + code, this.getHttpOptions());
  }
}

export class CountrySummary {
  countryName: string;
  region: string;
  capitalCity: string;
  longitude: string;
  latitude: string;
}
