import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import 'jasmine';
import { HomeComponent } from './home.component';
import { FormsModule } from '@angular/forms';
import { CountryLookupService, CountrySummary } from '../services/countrylookup.service';
import { Observable } from 'rxjs';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let mockCountryLookupService;

  beforeEach(() => {

    TestBed.configureTestingModule({
      providers: [CountryLookupService],
      declarations: [HomeComponent],
      imports: [FormsModule]
    })
      .compileComponents();

    mockCountryLookupService = jasmine.createSpyObj(['getValidCountryCodes', 'getCountryDetails']);
    mockCountryLookupService.getValidCountryCodes.and.returnValue(new Observable<string[]>());
    mockCountryLookupService.getCountryDetails.and.returnValue(new Observable<CountrySummary>());

    // Configure the component with another set of Providers
    TestBed.overrideComponent(
      HomeComponent,
      { set: { providers: [{ provide: CountryLookupService, useValue: mockCountryLookupService }] } }
    );

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;

    component.allValidCodesList = ['GB'];
    fixture.detectChanges();
  });

  it('should create as expected', () => {
    expect(component).toBeTruthy();
    expect(component.allValidCodesList).toContain('GB');
  });

  it('No list, not Invalid', () => {

    component.allValidCodesList = null;
    component.inputCode = 'g';

    expect(component.isInvalid()).toBeFalsy();
  });

  it('1 character is not Invalid', () => {
    component.inputCode = 'g';

    expect(component.isInvalid()).toBeFalsy();
  });

  it('2 characters Valid', () => {
    component.inputCode = 'gb';

    expect(component.isInvalid()).toBeFalsy();
  });

  it('2 uppercase characters Valid', () => {
    component.inputCode = 'GB';

    expect(component.isInvalid()).toBeFalsy();
  });

  it('2 characters Invalid', () => {
    component.inputCode = 'gc';

    expect(component.isInvalid()).toBeTruthy();
  });

  it('OnChange and input valid calls service once', () => {

    component.inputCode = 'gb';
    component.onChange();

    expect(mockCountryLookupService.getCountryDetails).toHaveBeenCalledTimes(1);
  });

  it('OnChange and one character input not valid calls service never', () => {

    component.inputCode = 'g';
    component.onChange();

    expect(mockCountryLookupService.getCountryDetails).toHaveBeenCalledTimes(0);
  });

  it('OnChange and two character input not valid calls service never', () => {

    component.inputCode = 'gc';
    component.onChange();

    expect(mockCountryLookupService.getCountryDetails).toHaveBeenCalledTimes(0);
  });

  it('1 character is not valid country code', () => {
    component.inputCode = 'g';

    expect(component.isValidCountryCode()).toBeFalsy();
  });

  it('2 characters valid country code', () => {
    component.inputCode = 'gb';

    expect(component.isValidCountryCode()).toBeTruthy();
  });

  it('2 uppercase characters valid country code', () => {
    component.inputCode = 'GB';

    expect(component.isValidCountryCode()).toBeTruthy();
  });

  it('2 characters invalid country code', () => {
    component.inputCode = 'gc';

    expect(component.isValidCountryCode()).toBeFalsy();
  });

  it('getInputUpper returns upper', () => {

    component.inputCode = 'aaa';
    const upper = component.getInputUpper();

    expect(upper).toBe('AAA');
  })

  it('getInputUpper handles null', () => {

    component.inputCode = null;
    const upper = component.getInputUpper();

    expect(upper).toBe(null);
  })
});

