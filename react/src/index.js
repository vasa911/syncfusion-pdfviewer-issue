import { render } from "react-dom";
import "./index.css";
import * as React from "react";
import {
  PdfViewerComponent,
  Toolbar,
  Magnification,
  Navigation,
  LinkAnnotation,
  BookmarkView,
  ThumbnailView,
  Print,
  TextSelection,
  TextSearch,
  Annotation,
  FormFields,
  Inject
} from "@syncfusion/ej2-react-pdfviewer";
import { SampleBase } from "./sample-base";
export class Default extends SampleBase {
  render() {
    return (
      <div>
        <div className="control-section">
          <PdfViewerComponent
            id="container"
            serviceUrl="http://localhost:5646/api/PdfViewer"
            style={{ height: "640px" }}
          >
            <Inject
              services={[
                Toolbar,
                Magnification,
                Navigation,
                LinkAnnotation,
                BookmarkView,
                ThumbnailView,
                Print,
                TextSelection,
                TextSearch,
                Annotation,
                FormFields
              ]}
            />
          </PdfViewerComponent>
        </div>
      </div>
    );
  }
}

render(<Default />, document.getElementById("sample"));
