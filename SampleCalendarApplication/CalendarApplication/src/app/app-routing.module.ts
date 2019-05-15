import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DemoComponent } from './components/demo/demo.component';
import { UploadComponent } from './components/upload/upload.component';

const routes: Routes = [
  {
    path: "demo", component: DemoComponent,
  },
  {
    path: "", component: UploadComponent, pathMatch: "full"
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
