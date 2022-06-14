import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeriranimaisComponent } from './geriranimais.component';

describe('GeriranimaisComponent', () => {
  let component: GeriranimaisComponent;
  let fixture: ComponentFixture<GeriranimaisComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GeriranimaisComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GeriranimaisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
