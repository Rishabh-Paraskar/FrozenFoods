(function() {
    var bindEvents = function() {
        $(document).on('click', '.dropdown', function(e) {
            // 关闭其他菜单
            $('.dropdown.open').not(e.currentTarget).removeClass('open')
            // 切换当前菜单
            $(e.currentTarget).toggleClass('open')

        })

        $(document).on('click', function(e) {
            // 点击空白处关闭
            var menu = $(e.target).closest('.dropdown')
            if (menu.length < 1) {
                $('.dropdown').removeClass('open')
            }
        })
    }

    var __main = function() {
        bindEvents()
    }

    $(document).ready(function() {
        __main()
    })
}())
