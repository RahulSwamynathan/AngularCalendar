import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DemoComponent } from './components/demo/demo.component';

const routes: Routes = [
  {
    path: "demo", component: DemoComponent,
  },
  {
    path: "", component: DemoComponent, pathMatch: "full"
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
