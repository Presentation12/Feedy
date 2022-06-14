import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnimaisgerenteComponent } from './animaisgerente.component';

describe('AnimaisgerenteComponent', () => {
  let component: AnimaisgerenteComponent;
  let fixture: ComponentFixture<AnimaisgerenteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AnimaisgerenteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AnimaisgerenteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
