import { MessageConstants } from './../../../shared/constants/message.constants';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, take, takeUntil } from 'rxjs';
import { AddPostSeriesRequest, AdminApiSeriesApiClient, PostInListDto } from './../../../api/admin-api.service.generated';
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
        this.loadData(this.config.data.id);
    }
    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubcribe.next();
        this.ngUnsubcribe.complete();
    }

    loadData(id: string) {
        this.toggleBlockUI(true);
        this.seriesService.getPostsInSeries(id).pipe(takeUntil(this.ngUnsubcribe)).subscribe({
            next: (res: PostInListDto[]) => {
                this.posts = res;
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    removePost(id: string) {
        this.toggleBlockUI(true);
        var body: AddPostSeriesRequest = new AddPostSeriesRequest({
            postId: id,
            seriesId: this.config.data.id
        });
        this.seriesService.deletePostSeries(body).pipe(takeUntil(this.ngUnsubcribe)).subscribe({
            next: () => {
                this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
                this.loadData(this.config.data.id);
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }
    private toggleBlockUI(enabled: boolean) {
        if (enabled == true) {
            this.blockedPanel = true;
        } else {
            setTimeout(() => {
                this.blockedPanel = false;
            }, 1000);
        }
    }
}