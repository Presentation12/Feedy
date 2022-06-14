import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FaltarComponent } from './faltar.component';

describe('FaltarComponent', () => {
  let component: FaltarComponent;
  let fixture: ComponentFixture<FaltarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FaltarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FaltarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
