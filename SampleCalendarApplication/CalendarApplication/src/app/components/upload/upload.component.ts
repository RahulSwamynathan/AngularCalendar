import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http'

@Component({
  selector: 'web-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent  {

  public progress: number;
  public message: string;

  constructor(private http: HttpClient) { }

  upload(files) {
    if (files.length === 0)
      return;
    const formData = new FormData();
    let fileName: string[] = [];
 
    for (let file of files) {
      formData.append(file.name, file);
      fileName.push(file.name);
    }

    //let obj = {
    //  userId: "rahulswa.rahul60@gma.com",
    //  fileNames: fileName
    //}

    //formData.append("key", JSON.stringify(obj));

    //const uploadReq = new HttpRequest('POST', `api/upload`, formData, {
    //  reportProgress: true,
    //});
    const uploadReq = new HttpRequest('POST', `/api/Upload`, formData, {
      reportProgress: true,
    });

    //this.http.request(uploadReq).subscribe((res) => {
    //  console.log(res);
    //}, (error) => {
    //  console.log(error);
    //  })

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response)
        this.message = event.body.toString();
    });
  }

}
