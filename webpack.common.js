const path = require('path');

module.exports = {
    entry: {
      editMap: './Scripts/EditMap.ts',
      miniMap: './Scripts/MiniMap.ts',
  },
  experiments: {
    outputModule: true,
  },
    output: {
        filename: 'js/[name].js',
      path: path.resolve(__dirname, 'wwwroot'),
      library: {
        type: "module",
      },
  },
    module: {
        rules: [
            {
                test: /\.([cm]?ts|tsx)$/,
                loader: 'ts-loader',
                exclude: /node_modules/,
            }
        ]
    },
}