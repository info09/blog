import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { AdminApiSeriesApiClient, PostInListDto } from './../../../api/admin-api.service.generated';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AlertService } from './../../../shared/services/alert.service';
@Component({
    templateUrl: "series-posts.component.html",
})

export class SeriesPostsComponent implements OnInit, OnDestroy {
    private ngUnsubcribe = new Subject<void>();
    public blockedPanel: boolean = false;
    public title: string;
    public posts: PostInListDto[] = [];

    constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig, private seriesService: AdminApiSeriesApiClient, private alertService: AlertService) { }

    ngOnInit(): void {
        throw new Error('Method not implemented.');
    }
    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubcribe.next();
        this.ngUnsubcribe.complete();
    }

}