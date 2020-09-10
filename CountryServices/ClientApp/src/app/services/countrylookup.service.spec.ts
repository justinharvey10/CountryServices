import { TestBed } from '@angular/core/testing';

import { CountryLookupService } from './countrylookup.service';

describe('CountrylookupserviceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CountryLookupService = TestBed.get(CountryLookupService);
    expect(service).toBeTruthy();
  });
});
