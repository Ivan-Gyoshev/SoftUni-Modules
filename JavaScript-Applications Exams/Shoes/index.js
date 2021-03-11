const userModel = firebase.auth();
const db = firebase.firestore();
const app = Sammy('#root',function() {

   this.use('Handlebars','hbs');
   
   //Home routes
   this.get('/home',function(context) {
        db.collection('offers').get()
        .then((res) => {
            context.offers = res.docs.map((offer) =>{ return {id: offer.id, ...offer.data()}});
          
                extendContext(context)
                .then(function(){
                    this.partial('./templates/home.hbs');
                })
        })
        .catch(errorHandler);     
   });
   //User routes
        //get-register
   this.get('/register',function(context) {
    extendContext(context)
    .then(function(){
        this.partial('./templates/register.hbs');
    })
   });
        //post-register
   this.post('/register', function(context){
      const {email, password, rePassword} = context.params;

      if (password !== rePassword) {
          return;
      }

      userModel.createUserWithEmailAndPassword(email, password)
      .then((userData) => {
          console.log(userData);
          this.redirect('#/home');
      })
      .catch(errorHandler);
   });
         //get-login
   this.get('/login',function(context) {
    extendContext(context)
    .then(function(){  
        this.partial('./templates/login.hbs')
    })
   });
         //post-login
   this.post('/login',function(context){

       const {email, password} = context.params;

       userModel.signInWithEmailAndPassword(email, password)
       .then((userData) => {
          saveUserData(userData);
          this.redirect('#/home')
       })
       .catch(errorHandler);
   })
   //Logiut 
   this.get('/logout',function(context){
        userModel.signOut()
        .then((res) => {
            clearUserData();
            this.redirect('#/login')
        })
        .catch(errorHandler);
   });
   //Offer routes
   this.get('/create-offer',function(context){
    extendContext(context)
    .then(function(){  
        this.partial('./templates/createOffer.hbs')
    })
   });
         //post offer
   this.post('/create-offer', function(context){
     const { productName , price , imageUrl, description , brand } = context.params;
     db.collection('offers').add({
        productName,
        price,
        imageUrl,
        description,
        brand,
        salesman: getUserData().uid,
        clients: []
     })
     .then((createdProduct) => {
         console.log(createdProduct);
         this.redirect('#/home');
     })
     .catch(errorHandler);
   });
   this.get('/edit-offer/:id',function(context){
    extendContext(context)
    .then(function(){  
        this.partial('./templates/editOffer.hbs')
    })
   });
  
   this.get('/details/:id',function(context){
    const { id } = context.params;
    db.collection('offers').doc(id).get()
    .then((res) => {
        const { uid } = getUserData();

        const actualOfferData = res.data();

        const imTheSalesMan = actualOfferData.salesman === uid;

        const index = actualOfferData.clients.indexOf(uid);

        const imInClientsList = index > -1;
         console.log(imInClientsList);

       context.offer = {...actualOfferData, imTheSalesMan ,id: id, imInClientsList};
        extendContext(context)
        .then(function(){  
            this.partial('./templates/offerDetails.hbs')
        })
    });
   });
 
   this.get('/delete/:id', function (context){
       const { id } = context.params;
       
       db.collection('offers').doc(id).delete()
       .then(() => {
        this.redirect('#/home')
       })
       .catch(errorHandler);
   })

   this.get('/edit/:id', function(context) {
       const { id } = context.params;
       console.log(id);
       db.collection('offers')
       .doc(id)
       .get()
       .then((res) => {
        context.offer = { id: id, ...res.data() };

        extendContext(context)
         .then(function () {
            this.partial('./templates/editOffer.hbs');
         });
       })
   });
   
   this.post('/edit/:id', function (context) {
    const { id,productName,price,imageUrl,description,brand} = context.params;
    
   db.collection('offers')
   .doc(id)
   .get()
   .then((res) => {
      return db.collection('offers')
       .doc(id)
       .set({
          ...res.data(),
          productName,
          price,
          imageUrl,
          description,
          brand
       })
    })
    .then((res) => {
        this.redirect(`#/details/${id}`);
    })
    .catch(errorHandler)
   });

   this.get('/buy/:id', function(context) {
       const { id } = context.params;
       const { uid } = getUserData();

       db.collection('offers')
       .doc(id)
       .get()
       .then((res) => {
            const offerData = {...res.data()};
            offerData.clients.push(uid)
            return db.collection('offers')
              .doc(id)
              .set(offerData)
       })
       .then(() => {
           this.redirect(`#/details/${id}`);
       })
       .catch(errorHandler);
   })

});

(() => {
    app.run('/home');
})();

function extendContext(context) {

   const user = getUserData();
   context.isLoggedIn = Boolean(user);
   context.email = user ? user.email : '' ;

    return context.loadPartials({
        'header': './partials/header.hbs',
        'footer': './partials/footer.hbs'
    })
}

function errorHandler(error){
    console.log(error);
}

function saveUserData(data){
    const { user: { email,uid } } = data;
     localStorage.setItem('user', JSON.stringify({email , uid}));
}

function getUserData(){
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
}

function clearUserData(){
    this.localStorage.removeItem('user');
}