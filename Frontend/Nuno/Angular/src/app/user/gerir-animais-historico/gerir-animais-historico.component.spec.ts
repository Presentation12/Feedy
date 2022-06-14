import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GerirAnimaisHistoricoComponent } from './gerir-animais-historico.component';

describe('GerirAnimaisHistoricoComponent', () => {
  let component: GerirAnimaisHistoricoComponent;
  let fixture: ComponentFixture<GerirAnimaisHistoricoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GerirAnimaisHistoricoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GerirAnimaisHistoricoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
