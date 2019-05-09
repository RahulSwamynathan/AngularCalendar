import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'web-calender-header',
  templateUrl: './calender-header.component.html',
  styleUrls: ['./calender-header.component.css']
})
export class CalenderHeaderComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  @Input() view: string;

  @Input() viewDate: Date;

  @Input() locale: string = 'en';

  @Output() viewChange: EventEmitter<string> = new EventEmitter();

  @Output() viewDateChange: EventEmitter<Date> = new EventEmitter();
}
