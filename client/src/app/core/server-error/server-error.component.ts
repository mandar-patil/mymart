import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.scss']
})
export class ServerErrorComponent implements OnInit {
  error: any;

  constructor(private router: Router) { 
    //navigation extras are available only in constructor
    const navigation = this.router.getCurrentNavigation();

    //optional chaining
    this.error = navigation?.extras?.state?.error;
  }

  ngOnInit(): void {
  }

}
