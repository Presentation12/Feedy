import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaberMaisComponent } from './saber-mais.component';

describe('SaberMaisComponent', () => {
  let component: SaberMaisComponent;
  let fixture: ComponentFixture<SaberMaisComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaberMaisComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SaberMaisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
