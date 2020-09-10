import { TestBed } from '@angular/core/testing';

import { CountrylookupserviceService } from './countrylookupservice.service';

describe('CountrylookupserviceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CountrylookupserviceService = TestBed.get(CountrylookupserviceService);
    expect(service).toBeTruthy();
  });
});
