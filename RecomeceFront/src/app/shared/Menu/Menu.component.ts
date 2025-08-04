import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-Menu',
  templateUrl: './Menu.component.html',
  styleUrls: ['./Menu.component.css']
})
export class MenuComponent implements OnInit {

  menuOpen = true;

  constructor() { }

  ngOnInit() {
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }
}
