import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewInventoriesComponent } from './view-inventories.component';

describe('ViewInventoriesComponent', () => {
  let component: ViewInventoriesComponent;
  let fixture: ComponentFixture<ViewInventoriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ViewInventoriesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ViewInventoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
