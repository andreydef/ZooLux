// Script to preview the images
$(function() {

    /* Preview selected image */
    function readUrl(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function(e) {
                $("img#imgpreview")
                    .attr("src", e.target.result)
                    .width(200)
                    .height(200);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    // Function, after page reloading for output the image
    $("#imageUpload").change(function() {
        readUrl(this);
    });

    /*-----------------------------------------------------------*/
});