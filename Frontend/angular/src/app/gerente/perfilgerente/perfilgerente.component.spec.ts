import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PerfilgerenteComponent } from './perfilgerente.component';

describe('PerfilgerenteComponent', () => {
  let component: PerfilgerenteComponent;
  let fixture: ComponentFixture<PerfilgerenteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PerfilgerenteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PerfilgerenteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
